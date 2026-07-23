# Shipment & Logistics Module

## Concept Overview
- Handles the physical fulfillment side of an order: shipment creation, courier selection, tracking history, and status progression through pickup → delivery.
- **ShipRocket** is integrated purely for **courier serviceability checking and rate estimation** (which courier can deliver this weight between these two postcodes, at what rate, in how many days) — it is used at order-placement time and return-shipment-creation time to select a courier and calculate shipping charge.
- **Shipment status progression itself is admin-driven, not courier-webhook-driven** — since real-time carrier pickup/delivery integration isn't available in this environment, an admin manually advances a shipment through its status lifecycle (Picked Up → Delivered, etc.) via a dedicated update endpoint, and the system reacts to each status change with the appropriate downstream effects (order item status, inventory release, notifications).

## ShipRocket Integration (`ShiprocketService`)
- Authenticates against ShipRocket's API (token-based login using configured credentials)
- `CheckServiceability` — given pickup postcode, delivery postcode, weight, and COD flag, calls ShipRocket's serviceability endpoint and parses the list of available courier companies
- **Best courier selection logic:** filters to couriers with a **rating ≥ 4**, then picks the one with the **fewest estimated delivery days**, tie-broken by **lowest rate** — this is what actually decides which courier gets assigned to a shipment, not manual admin choice
- Used both for normal forward shipments (at order placement) and return shipments (post return-approval)

## Core Shipment Building Blocks (`ShipmentService`)
- `CreateShipment` — creates the `Shipment` row from checkout/return data, audit-logged
- `CreateShipmentItems` — links individual `OrderItems` to a shipment
- `CreateShipmentTracking` — appends a tracking history entry (location, remarks, status) to a shipment's timeline
- All three are simple, reusable building blocks called by higher-level flows (order placement, return-shipment creation, admin status updates)

## Admin-Driven Shipment Status Update (`UpdateShimentStatus`)
Since there's no live courier webhook, this endpoint is the **manual substitute** for real-time carrier status updates — an admin selects a shipment and sets its new status, and the system cascades the correct downstream effects based on what that status means:

- **Status → `Picked_Up`:** `ShippedDate` set; all order items under this shipment moved to `Processed`. Order admins and the customer both notified ("Order Picked Up").
- **Status → `Delivered`, and it's a Return-type shipment:** internally re-mapped to `Returned` instead — `DeliveryDate` set, order items moved to `Returned`, and each related `Return` record's status flipped to `Received` (this is the "product arrived back at the vendor" trigger described in the Return Lifecycle module, feeding into the vendor's inspection step).
- **Status → `Delivered` (normal forward shipment):** `DeliveryDate` set; order items moved to `Delivered`. Order admins and customer notified. **Reserved inventory released** at this point (`ReservedQuantity` decremented) — stock reservation is only fully cleared once delivery is confirmed, not at pickup.
- Every status transition writes a `Shipment` audit log entry and a new `ShipmentTracking` entry (location + remarks supplied by the admin performing the update) so the shipment's full history is visible end-to-end.
- After updating order items to `Delivered`, `CheckIfAllOrderItemsShipped` re-evaluates the parent `Order`: if no items remain undelivered, the whole `Order` is marked `Completed`, with admin + customer notifications sent.

## Admin Shipment Listing & Lookup
- `GetAllShipmentsForAdmin` — paginated, filterable shipment list across the whole platform
- `GetShipmentDetailForAdmin` — full detail view for a single shipment by ID
- `GetShipmentDetailForOrderItemId` — look up a shipment via one of its order items (used by customer-facing order-detail pages to show tracking)

## Edge Cases Handled
- Best-courier selection filters out low-rated couriers (< 4 rating) before even considering speed/cost, so a cheap-but-unreliable courier is never auto-selected
- Return shipments and forward shipments share the same `Shipment`/`ShipmentTracking` infrastructure but are distinguished by `ShipmentTypeId`, so a "Delivered" status is interpreted differently depending on shipment type (forward → customer received it; return → vendor received it back)
- Inventory reservation is only released on confirmed delivery, not on pickup — protects against releasing stock too early while the item is still in transit
- Order only marked `Completed` once every item across every shipment under it has been delivered — partial delivery (multi-vendor orders) does not prematurely complete the order
- Every status change triggers both an order-admin notification and a customer notification, plus a permanent tracking-history entry, regardless of which status transition occurred
- Missing order items for a shipment ID is treated as an error (`DataNotFoundException`) rather than silently doing nothing, since a shipment with no items would indicate a data integrity problem
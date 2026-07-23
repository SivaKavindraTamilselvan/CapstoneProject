# Order Placement Workflow Module (Checkout Serviceability + Order Creation)

## Two-Stage Design
1. **`CheckService`** — a **dry-run** check called from the checkout review step, before the user actually confirms placing the order. Validates everything, calculates total shipping charge, and tells the frontend "yes, this is shippable, here's the cost" — **does not create any Order/Shipment records**.
2. **`AddOrder`** (built on the same validation/inventory/shipment building blocks) — the real order-placement flow, called only when the user actually submits. Creates `Order`, `OrderItems`, `Shipment`(s), reserves inventory, and proceeds to payment.

This split means the exact same validation and courier-selection logic runs twice — once as a preview, once for real — so the shipping estimate shown to the user during review always matches what's actually charged at placement.

## Step-by-Step: `CheckService` (Pre-Checkout Validation & Shipping Estimate)

1. **`ValidateProduct(userId)`** — fetches the user's cart; fails if empty, if any item's `ProductVariant` failed to load, or if `CheckTheProduct` reports the requested quantity isn't actually available for any SKU (out-of-stock/insufficient-stock guard, checked item by item).
2. **`ValidateAddress(addressId, userId)`** — address must exist and must belong to the requesting user (ownership check), not just exist.
3. **`CalculateProductCharge(cartItems)`** — sums `Quantity * Price` across all cart items.
4. **COD flag derived from payment method** — `cod = 1` if `PaymentMethod == 1` (Cash on Delivery), else `0`; this flag is passed into every serviceability check since COD affects courier availability/rates.
5. **`ValidateCoupon(userId, couponId)`** (only if a coupon was selected) — coupon must be in the user's list of currently-available coupons (`GetAllAvailableCoupons`), not just exist globally; a coupon the user isn't eligible for is rejected here.
6. **`GetTheInventoryPickupAddress(cartItems, deliveryAddress, cod)`** — for **each cart item individually**:
   - Filters that product variant's inventories down to ones with enough `AvailableQuantity` to fulfill the requested quantity
   - Checks courier serviceability (`ShipRocketService.CheckServiceability`) from **each candidate inventory location** to the delivery address
   - Picks whichever inventory location gives the **lowest shipping rate** among all valid courier-serviceable options — this is effectively picking the cheapest-to-ship warehouse for that specific item, not just the nearest one
   - Throws `DataNotFoundException("No courier available for product")` if no inventory location has both enough stock and courier coverage
7. **Group selected items by `(VendorId, PickupAddressId)`** — items going out from the same vendor and same warehouse address are batched into one shipment group (this is what determines how many separate shipments the order will eventually be split into).
8. **Per group:** sum the group's total weight, call `CalculateShippingCharge` (a fresh serviceability check for the *combined group weight*, pickup→delivery) to get that group's shipping cost, and add it to the running `totalShippingCharge`.
9. Returns `TotalShippingCharge` and `IsShippingAvailable = true` to the frontend for display in the checkout review step.

## Step-by-Step: Actual Order Placement (using the same building blocks)
- Same validation sequence as `CheckService` (cart, address, coupon) runs again for real.
- Same `GetTheInventoryPickupAddress` + vendor/pickup-address grouping logic determines the real shipment split.
- For each group: **`CreateShipment`** persists the `Shipment` row (`ShipmentTypeId = 1` = forward shipment, expected delivery date = `now + 2 + estimatedDeliveryDays`, courier name, shipping charge).
- **`CreateShipmentTracking`** immediately logs the first tracking entry ("Order Created Successfully. Item is in Warehouse") against the new shipment.
- `Order` and `OrderItems` created, tying each item to its resolved shipment.
- Inventory reserved (moved from available to reserved) for the resolved best-inventory location per item.
- Wallet deduction (if applicable) and payment flow proceed from there (see Orders & Wallet modules).

## Wallet Balance Lookup
- `GetWalletBalance(userId)` — simple direct lookup used by the checkout UI to display the user's current wallet balance before they decide whether to toggle "use wallet" at review time.

## Edge Cases Handled
- Empty cart, missing loaded product variant, or insufficient stock on any single item blocks the entire checkout at the validation stage, before any shipping/cost calculation runs
- Address ownership enforced — a user cannot check serviceability or place an order against an address belonging to someone else
- Coupon eligibility checked against the user's actual available-coupon list, not just coupon existence/validity in general
- Per-item inventory selection picks the cheapest serviceable warehouse, so a product stocked in multiple warehouses automatically ships from whichever location is cheapest for that specific delivery address
- Orders spanning multiple vendors and/or multiple pickup warehouses are automatically split into the correct number of separate shipments, each with its own courier, tracking, and shipping charge
- No courier available for a given product/route immediately fails the whole checkout with a clear reason, rather than silently omitting shipping cost
- The shipping estimate shown during review (`CheckService`) and the shipping charge actually applied at order placement are computed via the identical grouping/courier logic, so they should not diverge between preview and final submission
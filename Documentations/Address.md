# Address Management Module

## Concept Overview
- Manages both **customer delivery addresses** and **vendor pickup/inventory addresses** through the same underlying service, with policy-based endpoint separation.
- Vendor-side address management is restricted to a dedicated authorization policy (`VendorOnwerAndInventoryVendorOnly`) — separate from the general authenticated-user endpoints used by customers.

## Pincode Lookup (`GetPincode`)
- Integrates the **India Post Pincode API** (`https://api.postalpincode.in/pincode/{pincode}`) to auto-fill location details (post office, district, state) from just a 6-digit PIN code
- Called from the frontend address form so the user only needs to type a pincode and the rest of the location fields can be auto-populated/validated, rather than manually typing city/state/district themselves
- This endpoint is **unauthenticated** (no `[Authorize]`) since it's just a lookup proxy, not user data

## Customer Address Endpoints
- **Add Address** — creates a new address for the logged-in user
- **Make Address Default** — flips a specific address to be the user's default delivery address
- **Get Active Addresses** — lists all currently-active addresses belonging to the user
- **Get Single Address** — fetch one specific address by ID, scoped to the requesting user
- **Deactivate Address** — soft-deletes an address (PATCH, not a hard DELETE) — `DeleteAddress` action name is a bit misleading since it's actually a deactivation, not a row removal

## Vendor Address Endpoints
- **Get All Vendor Addresses** — paginated, filterable list of a vendor's own pickup/inventory addresses, restricted to Vendor Owner or Inventory-role vendor users only
- **Deactivate Vendor Address** — soft-deactivates a vendor's inventory address, same owner/inventory-role restriction

## User Identity Extraction Pattern
- Every authenticated endpoint pulls the acting user's ID directly from the JWT claims (`ClaimTypes.NameIdentifier`) rather than accepting it as a request parameter — this prevents a client from ever supplying someone else's user ID to act on their behalf; the ID always comes from the verified token.

## Edge Cases Handled
- Address ownership is enforced at the service layer for every customer-facing action (add/default/get/deactivate) — a user can only see or modify their own addresses
- Vendor address management is gated by a specific authorization policy rather than a general `[Authorize]`, preventing regular customers or non-inventory vendor roles from touching vendor pickup addresses
- Address deactivation is non-destructive (soft delete via status flag), preserving historical order/shipment records that reference the address
- Pincode lookup is a thin pass-through proxy to the external India Post API — no local caching or validation of the external response shown here, so its reliability depends on the third-party API's uptime
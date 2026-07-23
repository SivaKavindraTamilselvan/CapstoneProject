# Coupon Management Module

## Concept Overview
- **Coupons** — discount codes applied at checkout, reducing the order's final amount.
- **CouponTypeId** distinguishes who created the coupon: `Admin` (platform-wide coupon) vs `Vendor` (vendor-specific coupon) — the type field and split exist in the model, but **only the Admin-created path is currently implemented and in active use**; vendor-created coupons are not yet exposed as a working feature.
- **CouponsProduct** — an optional mapping restricting a coupon to specific products, rather than applying to the whole order. Not required for a basic overall-order-discount coupon.
- Coupon usage is tracked per order (`CouponUsageDto`/usage history) so a coupon's redemption count and history is auditable.

## Add Coupon
- Only Shipment-and-Coupon Admin role can create coupons
- Coupon code validated as unique before creation
- `CouponTypeId` set automatically based on the creator's role (Admin vs Vendor) — not user-selectable
- Creator's UserId recorded (`CreatedByUserId`)
- Audit log entry created (Created action)

## Activate Coupon
- Coupon must exist
- Cannot activate a coupon that's already active (guarded)
- Audit log entry created (old/new active state)

## Deactivate Coupon
- Coupon must exist
- Cannot deactivate a coupon that's already inactive (guarded)
- Audit log entry created (old/new active state)

## Example — Overall Order Discount Coupon
- **`WELCOME100`** — a flat discount coupon applied to the whole order's cost, not scoped to any specific product
- Configured with: `DiscountValue`, `MinimumOrderAmount` (order must meet this threshold to qualify), `StartDate`/`EndDate` (validity window), `MinimumNumberOfUsage`
- Since no `CouponsProduct` mapping is created for it, it applies platform-wide across any eligible order — this is the currently-implemented, working coupon pattern
- Product-specific coupons (via `RequestAddCouponProductDTO`/`CouponsProduct`) are modeled in the schema for future scoping (e.g. "10% off only on Electronics") but the overall-order-discount pattern is the one actually in use

## Admin Coupon Listing & Filters (`AdminCouponFilter`)
- `CouponId` / `Search` — direct lookup or text search
- `CouponTypeId` — filter Admin vs Vendor coupons (schema supports both even though only Admin path is live)
- `IsActive` / `IsExpired` — status filtering, computed expiry vs manual active flag kept as two separate concepts
- `ValidFrom` / `ValidTo` — validity window filtering
- `MinDiscountValue` / `MaxDiscountValue` — discount amount range
- `MinOrderAmount` / `MaxOrderAmount` — minimum-order-amount range filtering

## Coupon Detail View (`CouponDetailDto`)
- Full coupon configuration plus derived fields: `IsExpired`, `UsageCount`, `CreatedByUserName`
- `ApplicableProductIds` — populated only for product-scoped coupons; empty for overall-order coupons like WELCOME100
- `UsageHistory` — per-order redemption list (`CouponUsageDto`): which order, which user, order's final amount, and when it was used — gives admin a full audit trail of who redeemed the coupon and its actual revenue impact

## Edge Cases Handled
- Duplicate coupon codes blocked at creation
- Coupon cannot be activated if already active
- Coupon cannot be deactivated if already inactive
- Coupon type assigned server-side based on creator role, not client-supplied — prevents a vendor from creating an Admin-type coupon or vice versa
- `IsActive` (manual toggle) and `IsExpired` (date-derived) tracked as independent states — a coupon can be manually deactivated even while still within its date range, and a coupon can be expired by date while still flagged active, both reflected separately in filters and detail view
- Every coupon creation/activation/deactivation recorded in the audit log



# User Coupon Module

## Concept Overview
- Read-only, user-facing coupon retrieval — no mutations, no transactions, no audit logging (unlike the Cart/Favorites modules), since nothing is being created or changed here.
- Three query methods, all built around the same core idea: figure out what coupons a user is entitled to see or use, based on either global active status or their current cart contents.

## Get All Active Coupons
- User identity validated
- Delegates to `ValidateGetAllActiveCoupon(userId)` to fetch all currently active coupons relevant to the user
- Returns the mapped `ResponseGetAllCoupon` DTO list

## Get All Available Coupons (raw entity)
- User identity validated
- User's cart items resolved (`ValidateGetCartItemsByUserId`)
- **Cart cost calculated:** sum of `Quantity * ProductVariant.Price` across all cart items
- **Distinct product IDs extracted** from the cart (via each item's `ProductVariant.ProductId`) — used so coupon eligibility can be checked per-product, not per cart-line-item
- Delegates to `ValidateGetAllAvailableCoupons(cost, productId, userId)`, passing cart total, distinct product IDs, and user id so the validator can apply coupon rules (e.g. minimum spend, product-specific eligibility)
- Returns the **raw `Coupons` entity list** — no DTO mapping — distinguishing this from the next method

## Get All Available Coupons (user-facing DTO)
- Identical logic to `GetAllAvailableCoupons` above (same cart cost + distinct product ID calculation, same validator call)
- Only difference: the result is mapped to `List<ResponseGetAllCoupon>` before returning, making it suitable for direct API/client consumption
- Effectively a DTO-mapped wrapper over the same computation as `GetAllAvailableCoupons`

## Edge Cases Handled
- Coupon eligibility checks are product-based, not line-item based — `Distinct()` on `ProductId` avoids double-counting a product that appears as multiple cart items/variants
- Cart cost calculation is duplicated (not shared) between `GetAllAvailableCoupons` and `GetAllAvailableCouponsUser` — both compute cost/productId identically before diverging only on the return shape (entity vs DTO)
- No transaction wrapping is used anywhere in this service, consistent with it being read-only
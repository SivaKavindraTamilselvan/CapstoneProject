# Review Module

## Concept Overview
- Lets users leave a review on a specific `OrderItem` (not directly on a product), and lets anyone fetch an aggregated review summary for a given product.
- No transaction/audit-log wrapping — a single-entity create with no dependent-row updates, and a read-only aggregation query.

## Add Review
- Target order item validated (`ValidateOrderItem(OrderDetailsId)`)
- **Delivery guard:** review can only be added if `OrderItemStatusId == Delivered`; otherwise throws `DataApprovalStatusException` — prevents reviewing an item that hasn't actually been received
- Request DTO mapped directly to a `Reviews` entity and created via the repository
- Throws `DataNotFoundException` if creation fails
- Returns the mapped `ResponseAddReviewDTO`

## Get Product Review Summary
- Fetches all reviews for a product (`GetByProductId`)
- **Star breakdown:** builds a fixed 5→1 star histogram (`Dictionary<int,int>`), incrementing counts per review's `StarId` (unrecognized star values are silently ignored via `ContainsKey` check)
- **Average rating:** mean of all `StarId` values, rounded to 1 decimal place; defaults to `0` if there are no reviews (avoids divide-by-zero)
- Builds a full `ReviewItemDTO` list manually (not via AutoMapper) for each review, including:
  - `ReviewDescription` resolved from a related lookup table (`ReviewDescription.ReviewDescriptionName`), defaulting to empty string if null
  - `UserName` pulled through a three-level navigation chain: `OrderItems → Order → Users → FirstName`
- Returns a single `ProductReviewSummaryDTO` combining the product id, average, total count, star breakdown, and full review list

## Edge Cases Handled
- Reviews can't be added before an order item reaches `Delivered` status
- Star breakdown only counts recognized star values (1–5); any out-of-range `StarId` is skipped rather than throwing or corrupting the dictionary
- Average rating calculation guards against an empty review list to avoid a divide-by-zero / `Average()` exception
- Nullable navigation properties (`ReviewDescription`, `OrderItems`, `Order`, `Users`) are defensively null-checked with `?.` and `??` when building `UserName` and `ReviewDescription`, so a broken/missing relationship doesn't crash the summary — it just shows as null/empty instead
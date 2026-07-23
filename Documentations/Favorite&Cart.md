# Favorites (Wishlist) Module

## Concept Overview
- Each user has exactly one `Favorites` list (created automatically at registration — see Authentication module), containing many `FavoritesItems` rows, each pointing to a specific `ProductVariant`.
- Standard add/remove wishlist functionality, following the same transaction + audit-log pattern as every other mutating module in the platform.

## Add Favorite
- User identity validated
- User's own `Favorites` list resolved (`ValidateFavoriteByUserId`)
- **Duplicate guard:** validates the product variant isn't already in the user's favorites before adding — prevents the same item being favorited twice
- `FavoritesItems` row created, linking the favorites list to the product variant
- Audit log entry created (Created action)

## Delete Favorite
- User identity validated
- Target `FavoritesItems` row validated to exist (`ValidateFavoriteItems`)
- Row deleted
- Audit log entry created (Deleted action, old value recorded before removal)

## Edge Cases Handled
- Adding the same product variant to favorites twice is blocked at the validation step, not left to a database unique-constraint failure
- Both add and delete are fully transactional — a failure in the audit-log step rolls back the favorite add/delete itself, so the favorites list and its audit trail never fall out of sync
- Deleting a favorite validates the item actually exists first, rather than attempting a blind delete and silently succeeding/failing


# User Cart Module

## Concept Overview
- Each user has exactly one `Cart` (resolved via `ValidateCartByUserId`), containing many `CartItems` rows, each pointing to a specific `ProductVariant`.
- Follows the same transaction + audit-log pattern as every other mutating module in the platform.

## Add to Cart
- User identity validated
- User's own `Cart` resolved (`ValidateCartByUserId`)
- Product variant validated as approved (`ValidateProductVariantIfApproved`)
- **Duplicate guard:** validates the product variant isn't already in the user's cart before adding (`ValidateCartItemsByProductAndUser`)
- `Cart.UpdatedAt` bumped to `DateTime.UtcNow` and cart row updated
- `CartItems` row created, linking the cart to the product variant
- Audit log entry created (Created action)

## Update Cart Item (change quantity)
- User identity validated
- User's cart validated by user id
- Target `CartItems` row looked up by `CartItemsId`; throws `DataNotFoundException` if missing
- Parent `Cart` looked up by `CartId`; throws `DataNotFoundException` if missing
- Quantity updated on the `CartItems` row, old quantity captured for audit
- Audit log entry created for the `CartItems` change (Updated action)
- `Cart.UpdatedAt` also bumped to `DateTime.UtcNow`, cart row updated
- Second audit log entry created for the `Cart` change (Updated action)
- Both audit logs and both row updates happen inside the same transaction — either all four operations succeed or none do

## Delete Cart Item
- User identity validated
- User's cart validated by user id
- Target `CartItems` row validated to exist (`ValidateCartItems`)
- Row deleted
- Audit log entry created (Deleted action, old value recorded before removal)

## Delete All Cart Items (clear cart)
- User identity validated
- Audit log entry created up front, recording that a full clear occurred for the user (Deleted action)
- All cart items for the user deleted (`ValidateDeleteCartItemsByUserId`)
- **Nested-transaction aware:** checks `_ecommerceContext.Database.CurrentTransaction` first — if a transaction is already open (i.e. this method is being called from within another transactional operation), it reuses that transaction instead of starting a new one, and only commits/rolls back the transaction it itself started

## Edge Cases Handled
- Adding a product variant already in the cart is blocked at the validation step, not left to a database unique-constraint failure
- Only approved product variants can be added to a cart
- Updating a cart item validates both the `CartItems` row and its parent `Cart` exist before mutating either
- All operations (add, update, delete, delete-all) are fully transactional — a failure in the audit-log step rolls back the cart mutation itself, so the cart and its audit trail never fall out of sync
- `DeleteAllCart` avoids nested `BeginTransactionAsync` calls (which would fail against most providers) by detecting and reusing an already-open ambient transaction
- Deleting a single cart item validates the item actually exists first, rather than attempting a blind delete
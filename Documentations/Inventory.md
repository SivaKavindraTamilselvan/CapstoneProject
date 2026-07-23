# Inventory Module

## Concept Overview
- Manages per-address stock records (`Inventory`) for a vendor's product variants — each row ties a `ProductVariantId` to an `AddressId` with an `AvailableQuantity`.
- Vendor-facing: every mutation validates the acting vendor user, then checks the vendor itself is approved, before touching any inventory data.
- Follows the standard transaction + audit-log pattern, and additionally notifies the vendor's **owner** user on Add/Delete via SignalR-backed `_notificationService.SendToUser`.

## Add Inventory
- Vendor user validated (`ValidateInventoryVendorUserByUserId`)
- Vendor's approval status validated (`ValidateVendorIfApproved`)
- Product variant validated as belonging to this vendor (`ValidateProductVariant`)
- Address validated as belonging to this vendor user (`ValidateAddress`)
- Request DTO mapped to `Inventory` entity; throws `NullReferenceException` if mapping fails
- `Inventory` row created; throws `DataRegistrationException` if creation fails
- Audit log entry created (Created action)
- Owner vendor user resolved (`GetOwnerVendorUserByVendorId`); if found, sent an "New Inventory Added" notification — if not found, a warning is logged and the notification is skipped (not treated as an error)
- Transaction committed; returns mapped `ResponseAddInventoryDTO`

## Update Inventory (adjust quantity)
- Vendor user validated, vendor approval validated
- Target `Inventory` row fetched directly by id; throws `DataNotFoundException` if missing
- **Two update modes** driven by `UpdateType`:
  - `true` → **increment**: adds `AvailableQuantity` from the request to the existing stock
  - `false` → **decrement**: subtracts, but first checks current stock isn't lower than the requested decrement amount — throws `DataApprovalStatusException("Stock cannot be negative")` if it would go negative
- Updated row saved
- Audit log entry created (Updated action), recording previous vs new `AvailableQuantity`
- Transaction committed; returns mapped `ResponseUpdateInventoryDTO`
- No owner notification is sent for quantity updates (only Add/Delete notify)

## Delete Inventory (soft delete)
- Vendor user validated, vendor approval validated
- Target inventory validated to exist (`ValidateInventory`)
- Address tied to the inventory row validated as belonging to this vendor user
- **Soft delete:** `IsActive` flipped from `true` to `false` (row is not physically removed)
- Updated row saved; throws `DataRegistrationException` if the update fails
- Audit log entry created (Deleted action), recording old/new `IsActive` state
- Owner vendor user resolved; if found, sent an "Inventory Deleted" notification, same not-found-is-a-warning behavior as Add
- Transaction committed; returns mapped `ResponseUpdateInventoryDTO` (reuses the update DTO, since a soft delete is really just another state update)

## Edge Cases Handled
- Stock can never go negative — a decrement larger than current `AvailableQuantity` is rejected before it's applied
- Deleting inventory is a soft delete (`IsActive = false`), preserving historical stock records rather than losing them
- A missing "owner" vendor user (e.g. vendor has no designated owner) doesn't fail the whole operation — it's logged as a warning and the notification step is simply skipped
- Update fetches the inventory row directly (`Get`) rather than through a validator, but still explicitly null-checks and throws `DataNotFoundException`, unlike Add/Delete which go through `_inventoryValidation`

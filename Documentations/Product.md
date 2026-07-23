# Product Management Module — Vendor & Admin (Full Flow)

## Product Approval Status Lifecycle
| ID | Status | Meaning |
|---|---|---|
| 1 | Pending | Product created by a non-owner vendor user (e.g. Product Manager), awaiting vendor owner review |
| 2 | Vendor_Approved | Approved internally by the vendor (either auto-approved because the owner created it, or manually approved by the owner after review) — now waiting for admin review |
| 3 | Vendor_Rejected | Rejected internally by the vendor owner before ever reaching admin |
| 4 | Admin_Approved | Reviewed and approved by a Product Admin — product becomes live/sellable |
| 5 | Admin_Rejected | Reviewed and rejected by a Product Admin — vendor can revise and resubmit |
| 6 | Deleted_By_Admin | Product permanently taken down by admin (policy violation, poor reviews, etc.) — terminal state, cannot be edited or resubmitted |

## Full Flow — Vendor Adds a Product

**Two entry paths depending on who creates it:**
- **Vendor Owner creates the product** → approval status auto-set to `Vendor_Approved` (2) immediately — skips the internal review step, since the owner's own listing doesn't need the owner's own approval. Notification goes straight to **Product Admins** for external review.
- **Non-owner vendor user (e.g. Product Manager) creates the product** → approval status stays `Pending` (1). Notification goes to the **Vendor Owner**, not to admin — the product isn't visible to admin review yet until the owner internally approves it.

**Validations performed on add:**
- Vendor user is validated as an active, product-role-authorized user
- Vendor company itself must be approved before any product can be created under it
- Product's subcategory must exist and be active
- Audit log entry created (Created action)

## Full Flow — Vendor Updates a Product

Two distinct update paths, each with different rules:

**1. `UpdateProduct` — status-only update (e.g. marking Active/Inactive/Archived)**
- Vendor ownership validated (product must belong to the requesting vendor)
- Blocked entirely if the product is already `Archived` or `Deleted_By_Admin` — a deleted product cannot be touched again
- Only changes `ProductStatusId` (availability toggle), not approval workflow fields
- Audit log entry created (Updated action, old/new status recorded)

**2. `UpdateRejectedOrPendingProduct` — vendor owner revises product content**
- Only the **vendor owner** can perform this update (not a general product-role user)
- Blocked if the product is already `Admin_Approved` or `Deleted_By_Admin` — once admin has approved it, or once it's deleted, the vendor cannot silently edit the live/dead listing
- Meant specifically for fixing a **rejected** or still-**pending** product
- On successful update, approval status is reset to `Vendor_Approved` (2) — **resubmitting it for a fresh admin review**
- Product Admins notified that the product was modified and needs re-review
- Audit log entry created (Updated action, old/new approval status recorded)

## Full Flow — Admin Reviews a Product

- Admin can only review a product that is currently `Vendor_Approved` (2) — a product still `Pending` internal vendor review, or already reviewed (`Admin_Approved`/`Admin_Rejected`), is rejected outright
- Admin sets the new status to either `Admin_Approved` (4) or `Admin_Rejected` (5), with optional remarks
- **`ApprovalHistory` record created on every review** — captures `PreviousStatusId`, `NewStatusId`, `ReviewerId`, `ReviewerType`, `Remarks`, and which entity (`Product` / `Product_Variant`) was reviewed
- Vendor owner notified of the outcome — approval or rejection (with the admin's remarks included in the rejection message)
- Same review flow exists in parallel for `ProductVariant` (`ReviewProductVariant`), independently reviewable at the variant level

## Full Flow — Admin Deletes a Product

- Cannot delete a product that's already `Deleted_By_Admin` — guarded against double-deletion
- Deletion is a **soft delete**: `ProductApprovalStatusId` set to `Deleted_By_Admin` (6), not a row removal
- `ApprovalHistory` entry created recording the deletion with admin remarks (reason for deletion)
- Vendor owner(s) notified with the deletion reason
- Same soft-delete + approval-history + notification flow exists in parallel for `ProductVariant` (`DeleteProductVaraint`)

## Why Approval History Matters (Practical Use Case)
- Every review/delete action writes a permanent `ApprovalHistory` row — this becomes the vendor's **rejection reason trail**.
- When a product is `Admin_Rejected`, the vendor owner can open the product's approval history and see **exactly what the admin's remarks said** was wrong (e.g. "Description is misleading" / "Incorrect category selected" / "Image quality too low").
- The vendor then uses `UpdateRejectedOrPendingProduct` to fix precisely what was flagged, and on save the product automatically re-enters the `Vendor_Approved` queue for the admin to review again — **without needing to recreate the product from scratch** or lose the original listing's ID/history.
- This turns rejection into a correctable loop (Reject → see remarks → fix → resubmit → re-review) instead of a dead end, and gives admins a full audit trail of how many times a given product bounced back and forth and why.

## Vendor Product Filters (`RequestVendorProductFilter`)
Used on vendor's own product-list views, all fields optional and independently combinable:
- `ProductName` / `SearchTerm` — text search
- `ProductCategoryId` / `ProductSubCategoryId` — catalog scoping
- `ProductApprovalStatusId` — filter by any of the 6 lifecycle states above (e.g. show only Rejected products needing fixes)
- `ProductStatusId` — active/inactive/archived filter, independent of approval status
- `AddedByVendorUserId` — filter to products added by a specific vendor user (useful for an owner auditing a specific Product Manager's submissions)
- `MinPrice` / `MaxPrice` — price range
- `hasIssues` — flags products with outstanding validation problems
- `isAvailableForSale` — whether the product is currently purchasable
- `includeIsDeleted` — whether to include soft-deleted (`Deleted_By_Admin`) products in results, since these are normally excluded
- `MinAvailableQuantity` / `MaxAvailableQuantity` / `MinReservedQuantity` / `MaxReservedQuantity` — inventory-level filtering
- `MainProductSubCategoryAttributeId` — filter by the product's primary distinguishing attribute (e.g. all products where "Color" is the main attribute)

## Edge Cases Handled
- Owner-created products skip internal vendor review (auto-approved to `Vendor_Approved`) — only admin review remains
- Non-owner-created products require an explicit owner approval step before admin ever sees them
- A product cannot be status-updated once `Archived` or `Deleted_By_Admin`
- A product's content cannot be edited once `Admin_Approved` (locks a live listing from silent vendor changes) or once `Deleted_By_Admin`
- Admin cannot review a product that hasn't cleared vendor-internal approval yet
- Admin cannot re-review a product that's already been reviewed (Approved/Rejected) — prevents duplicate review actions
- Rejected/Pending products can be revised and are automatically resubmitted into the review queue on update
- Product and ProductVariant reviews/deletions are tracked as fully independent flows, each with their own approval history and notification path
- Missing parent Product reference during variant review/delete notification handled gracefully (logged and skipped) rather than throwing
- Every approval/rejection/deletion is permanently recorded in `ApprovalHistory`, giving vendors a clear, actionable trail of what to fix
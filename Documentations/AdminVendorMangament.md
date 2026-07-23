# Admin Vendor Management Module

## Concept Overview
- Admin-side control over vendor onboarding: reviewing a vendor's application (approve/reject), deleting a vendor, and browsing/searching all vendors and their sub-users platform-wide.
- **Only an Accepted (approved) vendor can log in / operate on the platform** — approval status directly gates vendor access, not just visibility in admin lists.

## Review Vendor (Approve / Reject)
- Vendor must exist
- Cannot review a vendor that's already been reviewed (`Accepted` or `Rejected` already set) — one-time decision, guarded against re-review
- Requesting admin validated as a Vendor Admin
- **`VendorApprovalHistory` entry created** — records previous status, new status, reviewer, remarks, and timestamp, giving a permanent audit trail of the review decision
- Vendor's `ApprovalStatusId`, `ReviewedByAdminId`, `ReviewedAt` updated
- Standard audit log (`LogChanges`) entry created alongside the approval-history entry
- **Vendor owner notified in-app** immediately after commit (Approved/Rejected title + remarks)
- **Vendor owner also emailed** at their `CompanyEmail`, separately from the in-app notification, after the transaction commits — approval/rejection outcome plus remarks in a formatted email
- Email send wrapped in its own try/catch — a failed email never rolls back the already-successful review
- Missing/blank `CompanyEmail` handled gracefully (logged warning, review still completes)

## Delete Vendor
- Requesting admin validated as a Vendor Admin
- Vendor must exist (404 if not)
- **`VendorApprovalHistory` entry created** recording the deletion as a status transition to `Deleted_By_Admin`, with admin remarks explaining the reason
- Vendor's `ApprovalStatusId` set to `Deleted_By_Admin` — **soft delete**, not a row removal
- Vendor owner notified in-app with the deletion reason
- Standard audit log entry created
- Notification is sent **before** the final `Update`/commit step in this method (unlike Review, where notification happens after commit) — worth confirming this ordering is intentional, since if the update step fails afterward, the vendor would already have been notified of a deletion that then rolled back

## Get Vendors (Admin, Paginated)
- Admin identity validated before fetch
- Paginated, filterable vendor list (`RequestAdminVendorFilter`)
- Empty result set returns 404, not an empty success list

## Get Single Vendor by ID (Admin)
- Admin identity validated before fetch
- Returns full vendor detail; 404 if vendor doesn't exist

## Get Vendor Users by Vendor (Admin, Paginated)
- Admin identity validated before fetch
- Paginated, filterable list of vendor sub-users (`RequestAdminVendorUserFilter`) — lets admin see all users under a given vendor company (owner + any product managers, order managers, etc.)

## Approval Status Gate on Login
- A vendor's `ApprovalStatusId` must be `Accepted` for that vendor (and by extension, its vendor users) to log in and use the platform
- A vendor sitting in `Pending`, `Rejected`, or `Deleted_By_Admin` cannot authenticate/operate, even if their `User`/`VendorUser` accounts technically exist — approval status is the actual access gate, separate from the account-level `IsActive` flag used elsewhere

## Edge Cases Handled
- Vendor cannot be reviewed twice — once Accepted or Rejected, the decision is locked
- Vendor deletion is non-destructive (soft delete via status), preserving all historical order/product data tied to that vendor
- Every review and deletion action is recorded twice: once in `VendorApprovalHistory` (decision-specific trail with remarks) and once in the general `LogChanges` audit table (consistent with every other module)
- Missing vendor owner user handled gracefully across both review and delete — notification skipped with a logged warning rather than failing the whole operation
- Missing company email handled gracefully during the review-outcome email step
- Only vendors with `Accepted` status can actually operate on the platform — rejected/pending/deleted vendors are visible to admin in listings but functionally locked out
# Admin User Management Module

## Concept Overview
- Manages Admin accounts themselves (distinct from managing catalog/orders/vendors) — creating new admins and activating/deactivating existing admin accounts.
- Every action in this module is restricted to the **Owner/Main Admin role only** — a regular admin (e.g. Product Admin, Order Admin) cannot create, activate, or deactivate other admin accounts.

## Register Admin
- Controller-level `AdminService.RegisterAdmin` is a thin wrapper: validates the caller is an admin, logs the request, then delegates the actual creation work to `AuthenticationService.RegisterAdmin` (the password-less invite flow described in the Authentication module)
- The underlying `AuthenticationService.RegisterAdmin` additionally validates that the **assigning admin** (`adminUserId`) exists as a real `AdminUser` before allowing the new admin to be created, and records `AssignedByAdminUserId` on the new `AdminUser` row — every admin account has a traceable record of which admin created it
- New admin receives the standard password-less invite email (set-password link, 48-hour token)

## Activate Admin User
- Only the **Owner Admin** can activate another admin account (`ValidateOwnerAdminUserByUserId`)
- Admin account must exist
- Cannot activate an admin that's already active (guarded)
- **Two records updated together, in one transaction:** the `AdminUser.IsActive` flag **and** the underlying `User.IsActive` flag — both must reflect the same active state, since a login check would look at the `User` record while admin-specific authorization looks at the `AdminUser` record
- Audit log entries created separately for both the `AdminUser` change and the `User` change

## Deactivate Admin User
- Same Owner-Admin-only restriction as activation
- Admin account must exist
- Cannot deactivate an admin that's already inactive (guarded)
- Same dual-update pattern: both `AdminUser.IsActive` and `User.IsActive` flipped together in one transaction
- Audit log entries created for both records

## Why Both `AdminUser` and `User` Are Updated Together
- `User` is the base account row shared by every role (used for login, password, contact info)
- `AdminUser` is the role-specific extension row (admin role type, who assigned them, admin-specific active flag)
- Deactivating only one of the two would leave the system in an inconsistent state — e.g. deactivating `AdminUser` alone but leaving `User.IsActive = true` could still allow the account to log in generally while being locked out of admin-specific screens, or vice versa
- Keeping both in the same transaction guarantees an admin is either fully active or fully inactive across the whole platform, never in a partial state

## Edge Cases Handled
- Only the Owner/Main Admin role can create, activate, or deactivate admin accounts — no self-service or peer-admin management
- New admin creation requires a valid, existing assigning admin — recorded for accountability
- Admin cannot be activated if already active
- Admin cannot be deactivated if already inactive
- Activation/deactivation always updates the linked `User` row in the same transaction as the `AdminUser` row, preventing a mismatched active state between the two
- Every activate/deactivate action is fully audit-logged on both affected tables
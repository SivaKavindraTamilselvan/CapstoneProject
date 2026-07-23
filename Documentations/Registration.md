# Authentication Module

## Concept Overview
- Handles registration for all four account types: User (customer), Admin, Vendor (company + owner), Vendor User (sub-user under an existing vendor).
- Two distinct password-handling paths depending on account type:
  - **Customer self-registration** (`Register`) — customer sets their own password immediately at signup.
  - **Admin / Vendor / Vendor-User registration** (`RegisterAdmin`, `RegisterVendor`, `RegisterVendorUser`) — password-less invite flow; account is created without a usable password, and the person sets their own password later via an emailed link.

## Password Hashing
- HMACSHA256 used for password hashing, with a **per-user randomly generated key** (`user.HashedKey`) stored alongside the hash — not a single shared server-wide key
- Password compared/verified using the stored per-user key at login time
- For password-less accounts, a random throwaway password (`Guid.NewGuid()`) is hashed and stored at creation time purely as a placeholder, since `IsPasswordSet = false` marks the account as not yet usable for login until the real password is set

## Customer Self-Registration (`Register`)
- Validates user details (email uniqueness, required fields, etc.)
- Creates the `User` row with the customer's own chosen password, hashed
- Automatically creates an empty **Cart** for the new user
- Automatically creates an empty **Favorites** list for the new user
- Both Cart and Favorites creation are wrapped in the same transaction as the user creation — a failure in either rolls back the whole registration
- Audit log entries created for User, Cart, and Favorites creation

## Password-less Invite Registration (Admin / Vendor / Vendor-User)
- Shared building block: `RegisterUserWithoutPassword` — creates the `User` row with `IsPasswordSet = false` and a throwaway hashed password, so the account exists but cannot be logged into yet
- After the role-specific entity is created (`AdminUser` / `Vendor`+`VendorUser` / `VendorUser`), a **`PasswordSetToken`** is generated:
  - Random GUID token
  - 48-hour expiry
  - `IsUsed = false`
- An email is sent containing a "set your password" link with the token, **after** the transaction commits, in its own try/catch — a failed email send is logged but does not roll back the otherwise-successful account creation
- Audit log entries created for every entity touched: User, role-specific entity (AdminUser/Vendor/VendorUser), and the PasswordSetToken itself

## Setting the Password via Emailed Token (`SetPassword`)
- Token looked up by its raw value
- Rejected if the token doesn't exist, has already been used, or has expired (past `ExpiresAt`)
- On success: user's password is hashed and stored (with a fresh per-user HMAC key), `IsPasswordSet` flipped to `true`, token marked as used (single-use enforced)
- Audit log entry created recording the `IsPasswordSet` state change

## Registration Variants Summary
| Method | Creates | Password Flow | Extra Entities Created |
|---|---|---|---|
| `Register` | Customer User | Sets password immediately | Cart, Favorites |
| `RegisterAdmin` | Admin User | Password-less invite | AdminUser, PasswordSetToken |
| `RegisterVendor` | Vendor company + its Owner user | Password-less invite | Vendor, VendorUser (Owner role), PasswordSetToken |
| `RegisterVendorUser` | Additional Vendor sub-user | Password-less invite | VendorUser (under existing vendor), PasswordSetToken |

## Edge Cases Handled
- Email invite failures never roll back a successful account/entity creation — isolated in their own try/catch
- Password-set tokens are single-use (`IsUsed` flag) and time-limited (48-hour expiry)
- Expired or already-used tokens rejected with specific exceptions (`TokenExpiredException`, `InvalidTokenException`) distinguishing the two failure reasons
- Every password hash uses its own randomly generated key, not a shared static key, so a key leak/rotation doesn't compromise every account at once
- Customer registration is atomic across User + Cart + Favorites — a customer never ends up with a user account but no cart
- Vendor-user and Admin invite flows both reuse the exact same `RegisterUserWithoutPassword` + token + email pattern, keeping password-less onboarding consistent across roles
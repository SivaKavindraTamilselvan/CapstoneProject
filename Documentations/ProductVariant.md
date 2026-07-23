# Product Variant Management Module — Vendor (with Admin Approval Gate)

## Concept Overview
- **Product** — the parent listing (name, description, subcategory, main attribute).
- **ProductVariant** — a specific sellable version of a product (its own SKU, price, weight/dimensions, stock via Inventory). One Product can have many ProductVariants.
- **ProductVariantAttribute** — one attribute value attached to a specific variant (e.g. "Color = Red" on this exact SKU), linked back to an admin-approved `ProductSubCategoryAttribute` mapping.
- A variant is effectively the product's **"sub-variant"** in practice — e.g. a T-shirt Product can have multiple ProductVariants for Size×Color combinations, each with its own SKU, price, and stock.

## Precondition: Parent Product Must Already Be Admin-Approved
- `AddProductVariant` calls `ValidateProductIfApproved(ProductId)` — a variant **cannot be created at all** unless the parent Product has already cleared full admin approval.
- This is stricter than the Product creation flow itself, and intentional: a Product's core identity (name, category, main attribute) must be locked in and approved before vendors are allowed to start listing sellable SKUs under it.

## Full Flow — Vendor Adds a Product Variant
- Vendor user validated (product role, vendor company approved)
- Parent Product must exist, be **admin-approved**, and belong to the requesting vendor
- Variant attributes list **cannot be empty** — at least one attribute must be supplied
- **The Product's `MainProductSubCategoryAttributeId` value must be present** in the submitted attribute list — a variant cannot be created without supplying a value for the product's designated main attribute
- Duplicate attributes (same `ProductSubCategoryAttributeId` submitted twice) rejected
- SKU auto-generated (`PV-{ProductId:D6}-{RandomCode}`), guaranteed unique via a regenerate-until-unique loop
- **Same owner-vs-non-owner approval split as Products:**
  - Vendor **Owner** adds the variant → auto-set to `Vendor_Approved`, notification goes straight to **Product Admins**
  - Non-owner vendor user adds the variant → stays `Pending`, notification goes to the **Vendor Owner** for internal review first
- Each submitted attribute is persisted individually via `AddProductVariantAttribute`, looping through the list
- Audit log entries created for both the variant and each attribute

## Full Flow — Add a Single Product Variant Attribute (`AddProductVariantAttribute`)
- Validates the vendor user, the target variant (ownership-scoped), and the parent product
- Validates the attribute being added is actually mapped to the product's subcategory (`ValidateProductSubCategoryAttribute`) — enforces the same category-mapping rule described in the Attribute/Mapped Attribute module
- On **update** (`updation = true`): checks the attribute isn't already added to this variant (duplicate guard), separate from the create-time duplicate check
- Notifies the vendor owner that a new attribute needs review
- Audit log action recorded as Created or Updated depending on the `updation` flag

## Full Flow — Vendor Updates a Product Variant

**1. `UpdateProductVariant` — status-only update**
- Updates only `ProductVariantStatusId` (e.g. Active/Inactive)
- Ownership-validated via `ValidateProductVariant`
- Audit log entry created (old/new status)

**2. `UpdateRejectedProductVariant` — owner revises variant content**
- Only the **vendor owner** can perform this
- Blocked if the variant is already `Admin_Approved` or `Deleted_By_Admin`
- On update, approval status reset to `Vendor_Approved` — resubmitted for a fresh admin review
- Product Admins notified of the modification
- Audit log entry created (old/new approval status)

## Highlighting the Role of `MainProductSubCategoryAttributeId`
- Every `Product` declares one attribute as its **main distinguishing attribute** (`MainProductSubCategoryAttributeId`) at the product level — e.g. for a T-shirt product, the main attribute might be **Size**.
- **Every variant created under that product must supply a value for that exact main attribute** — this is enforced explicitly in `AddProductVariant` before any other attribute is processed.
- This guarantees that no matter how many other attributes (Color, Material, Pattern) a variant carries, the catalog can always answer "what are all the Size options for this T-shirt?" reliably, because Size is guaranteed present on every single variant of that product.
- **Practical example:**
  - Product: **"Classic Cotton T-Shirt"**, main attribute = **Size**
  - Variant 1 (sub-variant): Size = M, Color = Blue → SKU `PV-000123-A1B2C3D4`
  - Variant 2 (sub-variant): Size = L, Color = Blue → SKU `PV-000123-E5F6G7H8`
  - Variant 3 (sub-variant): Size = M, Color = Red → SKU `PV-000123-I9J0K1L2`
  - Every one of these three variants **must** carry a Size value (the main attribute) — Color is a secondary/optional attribute on top of it. The frontend product page uses the main attribute to build the primary selector (e.g. Size buttons), with other attributes shown as secondary specs.
- Without this rule, a vendor could create a variant with only Color set and no Size, breaking the "pick your size" selector entirely for that one SKU.

## Edge Cases Handled
- Variant creation blocked entirely if the parent product isn't yet admin-approved
- Variant creation blocked if no attributes are supplied at all
- Variant creation blocked if the product's main attribute isn't included in the submitted attributes
- Duplicate attribute submissions (same attribute twice in one request) rejected
- Duplicate attribute already-added check applied separately during attribute updates
- SKU collisions avoided via a regenerate-until-unique loop
- Owner-added variants skip internal vendor review; non-owner-added variants require owner approval first, mirroring the Product-level flow exactly
- A variant cannot be content-edited once `Admin_Approved` or `Deleted_By_Admin` — only status (Active/Inactive) remains updatable at that point
- Rejected/Pending variants can be revised by the owner and are automatically resubmitted into the admin review queue
- Attribute values are always validated against the product's actual subcategory mapping — a variant can never carry an attribute that isn't approved for that subcategory
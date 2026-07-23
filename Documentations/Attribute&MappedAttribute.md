# Admin Module — Product Attribute & Mapped Attribute (SubCategory Attribute)

## Concept Overview
- **Attribute (AttributeMaster)** — a master list of generic product properties (e.g. RAM, Color, Size, Storage, Material). Created once, reusable across many subcategories.
- **Mapped Attribute (ProductSubCategoryAttribute)** — the link between one Attribute and one SubCategory, declaring "this attribute is valid/applicable for products in this subcategory." A vendor can only assign an attribute to a product if that attribute is mapped to the product's subcategory.
- This two-layer design (generic Attribute + explicit SubCategory mapping) is what prevents nonsensical attribute-product combinations across the catalog.

## Add Attribute
- Validates requesting user is a Product Admin
- Validates attribute name is not already registered (duplicate check)
- Records which admin added the attribute
- Audit log entry created (Created action)
- Notifies all vendor users that a new attribute exists

## Add Mapped Attribute (ProductSubCategoryAttribute)
- Validates requesting user is a Product Admin
- Validates both the Attribute and the SubCategory exist and are valid
- Validates this exact Attribute + SubCategory combination is not already mapped (duplicate mapping check)
- Records which admin created the mapping
- Audit log entry created (Created action)
- Notifies all vendor users that a new attribute mapping is available

## Activate / Deactivate Attribute
- Existence check before activate/deactivate
- Already-active / already-inactive guard prevents duplicate state changes
- Audit log entry created (Updated action, old/new active state recorded)
- Notifies all vendor users to review their products when an attribute's availability changes

## Activate / Deactivate Mapped Attribute (ProductSubCategoryAttribute)
- Existence check before activate/deactivate
- Already-active / already-inactive guard prevents duplicate state changes
- Audit log entry created (Updated action, old/new active state recorded)
- Notifies all vendor users to review their products when a mapping's availability changes

## Why the Mapping Layer Matters — Example
- Admin creates the attribute **"RAM"** in AttributeMaster — this alone does not attach RAM to anything yet.
- Admin maps **RAM → "Laptops & Computers" subcategory** via ProductSubCategoryAttribute.
- A vendor adding a laptop can now select RAM as one of the product's attributes, because the subcategory-attribute mapping exists.
- **Without the mapping step**, a vendor could otherwise attach RAM to *any* product in *any* subcategory — for example, a vendor selling **school bags** would be able to add "RAM: 8GB" as a bag attribute, which is nonsensical.
- The mapping layer forces every attribute-to-product assignment to first be explicitly approved for that specific subcategory by an admin, so the catalog stays logically consistent (bags get Size/Material/Color; laptops get RAM/Storage/Processor; RAM never appears on a bag).

## Edge Cases Handled
- Duplicate attribute names blocked at creation
- Duplicate Attribute+SubCategory mapping blocked at creation
- Attribute/Mapping cannot be activated if already active
- Attribute/Mapping cannot be deactivated if already inactive
- Every state change (create/activate/deactivate) writes an audit log before notifying vendors
- Vendors notified to review their existing products whenever an attribute or its subcategory mapping changes availability, since a deactivation could make an existing product's stored attribute value invalid
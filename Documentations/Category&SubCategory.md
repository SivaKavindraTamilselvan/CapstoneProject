# Admin Module — Product Category & SubCategory Management

## Get All Product Categories (Admin)
- Paginated, filterable list of categories (filter by category ID, status)
- Admin identity validated before fetch
- Empty result set returns 404 Not Found, not an empty success list

## Get All Product SubCategories (Admin)
- Paginated, filterable list of subcategories (filter by subcategory ID, category ID, status)
- Same admin validation and empty-result-404 pattern as category list

## Add Product Category
- Validates requesting user is a Product Admin
- Validates category name is not already registered (duplicate check)
- Records which admin added the category
- Full audit log entry created (Created action)
- Notifies all vendor users that a new category is available

## Add Product SubCategory
- Validates requesting user is a Product Admin
- Validates parent category exists and is active before allowing a subcategory under it
- Validates subcategory name is not already registered
- Stores commission percentage at creation
- Full audit log entry created (Created action)
- Notifies all vendor users of the new subcategory

## Activate Product Category
- Validates category exists
- Validates category is not already active
- Audit log entry created (Updated action, old/new active state recorded)
- Notifies all vendor users of activation

## Activate Product SubCategory
- Same guard pattern as category activation (existence check + already-active check)
- Audit log entry created
- Notifies all vendor users of activation

## Deactivate Product Category
- Validates category exists
- Audit log entry created (Updated action, old/new active state recorded)
- Notifies all vendor users of deactivation

## Deactivate Product SubCategory
- Same pattern as category deactivation
- Audit log entry created
- Notifies all vendor users of deactivation

## Edge Cases Handled
- Duplicate category/subcategory names blocked at creation
- SubCategory cannot be created under a non-existent or inactive category
- Category/SubCategory cannot be activated if already active
- Every state change (create/activate/deactivate) writes an audit log before notifying vendors
- Empty paginated list results return 404 instead of an empty success response
- All operations scoped behind admin identity validation
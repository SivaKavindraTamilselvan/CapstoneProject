import { Routes } from '@angular/router';
import { RegisterAdmin } from '../admin/admin-user/register-admin/register-admin';
import { AdminList } from '../admin/admin-user/admin-list/admin-list';
import { AdminProduct } from '../admin/admin-product/admin-product/admin-product';
import { ActivateAdmin } from '../admin/admin-user/activate-admin/activate-admin';
import { DeactivateAdmin } from '../admin/admin-user/deactivate-admin/deactivate-admin';
import { ReviewVendor } from '../admin/admin-vendor/review-vendor/review-vendor';
import { VendorList } from '../admin/admin-vendor/vendor-list/vendor-list';
import { DeleteVendor } from '../admin/admin-vendor/delete-vendor/delete-vendor';
import { ReviewProduct } from '../admin/admin-product/review-product/review-product';
import { DeleteProduct } from '../admin/admin-product/delete-product/delete-product';
import { CategoryList } from '../admin/admin-category/category-list/category-list';
import { ActiveCategory } from '../admin/admin-category/active-category/active-category';
import { InactiveCategory } from '../admin/admin-category/inactive-category/inactive-category';
import { Subcategorylist } from '../admin/admin-subcategory/subcategorylist/subcategorylist';
import { InactiveSubCategory } from '../admin/admin-subcategory/inactive-sub-category/inactive-sub-category';
import { ActiveSubCategory } from '../admin/admin-subcategory/active-sub-category/active-sub-category';
import { AttributeList } from '../admin/admin-attribute/attribute-list/attribute-list';
import { ActiveAttribute } from '../admin/admin-attribute/active-attribute/active-attribute';
import { InactiveAttribute } from '../admin/admin-attribute/inactive-attribute/inactive-attribute';
import { MappedAttributeList } from '../admin/admin-mapped-attribute/mapped-attribute-list/mapped-attribute-list';
import { ActiveMappedAttribute } from '../admin/admin-mapped-attribute/active-mapped-attribute/active-mapped-attribute';
import { InactiveMappedAttribute } from '../admin/admin-mapped-attribute/inactive-mapped-attribute/inactive-mapped-attribute';
import { AdminDetailProduct } from '../admin/admin-product/admin-detail-product/admin-detail-product';
import { AdminUserDetail } from '../admin/admin-user/admin-user-detail/admin-user-detail';
import { VendorDetails } from '../admin/admin-vendor/vendor-details/vendor-details';

export const AdminRoutes: Routes = [
    {
        path: 'users/register',
        component: RegisterAdmin
    },
    {
        path: 'users/list',
        component: AdminList
    },
    {
        path: 'users/activate',
        component: ActivateAdmin
    },
    {
        path: 'users/deactivate',
        component: DeactivateAdmin
    },
    {
        path: 'users/:id',
        component: AdminUserDetail
    },
    {
        path: 'vendors/review',
        component: ReviewVendor
    },
    {
        path: 'vendors/list',
        component: VendorList
    },
    {
        path: 'vendors/delete',
        component: DeleteVendor
    },
    {
        path: 'vendors/:id',
        component: VendorDetails
    },
    {
        path: 'products/list',
        component: AdminProduct
    },
    {
        path: 'product-details/:id',
        component: AdminDetailProduct
    },
    {
        path: 'products/review',
        component: ReviewProduct
    },
    {
        path: 'products/delete',
        component: DeleteProduct
    },
    {
        path: 'product-category/list',
        component: CategoryList
    },
    {
        path: 'product-category/list?status=active',
        component: ActiveCategory
    },
    {
        path: 'product-category/list?status=inactive',
        component: InactiveCategory
    },
    {
        path: 'product-sub-category/list',
        component: Subcategorylist
    },
    {
        path: 'product-sub-category/list?status=active',
        component: ActiveSubCategory
    },
    {
        path: 'product-sub-category/list?status=inactive',
        component: InactiveSubCategory
    },
    {
        path: 'product-attribute/list',
        component: AttributeList
    },
    {
        path: 'product-attribute/list?status=active',
        component: ActiveAttribute
    },
    {
        path: 'product-attribute/list?status=inactive',
        component: InactiveAttribute
    },
    {
        path: 'attribute-mapping/list',
        component: MappedAttributeList
    },
    {
        path: 'attribute-mapping/list?status=active',
        component: ActiveMappedAttribute
    },
    {
        path: 'attribute-mapping/list?status=inactive',
        component: InactiveMappedAttribute
    },
];
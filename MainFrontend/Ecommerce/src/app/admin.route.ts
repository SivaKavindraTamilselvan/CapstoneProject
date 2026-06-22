import { Routes } from '@angular/router';
import { RegisterAdmin } from './admin/admin-user/register-admin/register-admin';
import { AdminList } from './admin/admin-user/admin-list/admin-list'; 
import { AdminProduct } from './admin/admin-product/admin-product/admin-product';
import { ActivateAdmin } from './admin/admin-user/activate-admin/activate-admin'; 
import { DeactivateAdmin } from './admin/admin-user/deactivate-admin/deactivate-admin'; 
import { ReviewVendor } from './admin/admin-vendor/review-vendor/review-vendor';
import { VendorList } from './admin/admin-vendor/vendor-list/vendor-list';
import { DeleteVendor } from './admin/admin-vendor/delete-vendor/delete-vendor';
import { ReviewProduct } from './admin/admin-product/review-product/review-product';
import { DeleteProduct } from './admin/admin-product/delete-product/delete-product';
import { CategoryList } from './admin/admin-category/category-list/category-list';
import { ActiveCategory } from './admin/admin-category/active-category/active-category';
import { InactiveCategory } from './admin/admin-category/inactive-category/inactive-category';

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
        path : 'products/list',
        component : AdminProduct
    },
    {
        path : 'products/review',
        component : ReviewProduct
    },
    {
        path : 'products/delete',
        component : DeleteProduct
    },
    {
        path : 'product-category/list',
        component : CategoryList
    },
    {
        path : 'product-category/list?status=active',
        component : ActiveCategory
    },
    {
        path : 'product-category/list?status=inactive',
        component : InactiveCategory
    },
    
];
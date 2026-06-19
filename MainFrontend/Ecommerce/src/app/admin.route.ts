import { Routes } from '@angular/router';
import { RegisterAdmin } from './admin-user/register-admin/register-admin';
import { AdminList } from './admin-user/admin-list/admin-list';
import { AdminProduct } from './admin/admin-product/admin-product/admin-product';
import { ActivateAdmin } from './admin-user/activate-admin/activate-admin';
import { DeactivateAdmin } from './admin-user/deactivate-admin/deactivate-admin';
import { ReviewVendor } from './admin/admin-vendor/review-vendor/review-vendor';
import { ActiveVendor } from './admin/admin-vendor/active-vendor/active-vendor';
import { VendorList } from './admin/admin-vendor/vendor-list/vendor-list';
import { DeleteVendor } from './admin/admin-vendor/delete-vendor/delete-vendor';

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
        path: 'vendors/active',
        component: ActiveVendor
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
];
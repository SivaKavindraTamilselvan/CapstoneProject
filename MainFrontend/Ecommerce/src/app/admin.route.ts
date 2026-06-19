import { Routes } from '@angular/router';
import { RegisterAdmin } from './admin-user/register-admin/register-admin';
import { AdminList } from './admin-user/admin-list/admin-list';
import { AdminProduct } from './admin/admin-product/admin-product/admin-product';
import { ActivateAdmin } from './admin-user/activate-admin/activate-admin';
import { DeactivateAdmin } from './admin-user/deactivate-admin/deactivate-admin';

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
        path: 'users/activate',
        component: ActivateAdmin
    },
    {
        path : 'products/list',
        component : AdminProduct
    },
];
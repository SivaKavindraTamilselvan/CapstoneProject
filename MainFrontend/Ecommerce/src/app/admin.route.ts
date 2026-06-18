import { Routes } from '@angular/router';
import { RegisterAdmin } from './admin-user/register-admin/register-admin';
import { AdminList } from './admin-user/admin-list/admin-list';

export const AdminRoutes: Routes = [
    {
        path: 'users/register',
        component: RegisterAdmin
    },
    {
        path: 'users/list',
        component: AdminList
    },
];
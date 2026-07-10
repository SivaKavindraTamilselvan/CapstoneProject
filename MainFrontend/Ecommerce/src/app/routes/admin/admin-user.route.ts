import { Routes } from "@angular/router";
import { RegisterAdmin } from "../../admin/admin-user/register-admin/register-admin";
import { AdminList } from "../../admin/admin-user/admin-list/admin-list";
import { AdminUserDetail } from "../../admin/admin-user/admin-user-detail/admin-user-detail";


export const AdminUserRoutes: Routes = [
    {
        path: 'users/register',
        component: RegisterAdmin
    },
    {
        path: 'users/list',
        component: AdminList,
        data: { status: null, title: 'Admin List' }
    },
    {
        path: 'users/activate',
        component: AdminList,
        data: { status: true, title: 'Active Admins' }
    },
    {
        path: 'users/deactivate',
        component: AdminList,
        data: { status: false, title: 'Inactive Admins' }
    },
    {
        path: 'users/:id',
        component: AdminUserDetail
    },

];
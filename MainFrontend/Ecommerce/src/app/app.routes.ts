import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Register } from './register/register';
import { AdminLayout } from './admin-layout/admin-layout';
import { AdminRoutes } from './admin.route';

export const routes: Routes = [
    { path: '', component: Login },
    { path: 'register', component: Register },
    { path: 'admin', component: AdminLayout, children: AdminRoutes }
];

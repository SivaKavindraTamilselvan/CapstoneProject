import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Register } from './register/register';
import { AdminLayout } from './admin-layout/admin-layout';
import { AdminRoutes } from './admin.route';
import { RegisterVendor } from './register-vendor/register-vendor';

export const routes: Routes = [
    { path: '', component: Login },
    { path: 'register', component: Register },
    { path: 'register-vendor', component: RegisterVendor },
    { path: 'admin', component: AdminLayout, children: AdminRoutes }
];

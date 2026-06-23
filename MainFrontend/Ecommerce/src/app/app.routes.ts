import { Routes } from '@angular/router';
import { Login } from './authentication/login/login';
import { Register } from './authentication/register/register';
import { AdminLayout } from './admin-layout/admin-layout';
import { AdminRoutes } from './admin.route';
import { RegisterVendor } from './authentication/register-vendor/register-vendor';
import { UserProduct } from './user-product/user-product';
import { UserNavbar } from './user-navbar/user-navbar';
import { CategoryMobile } from './category-mobile/category-mobile';
import { SubCategoryMobile } from './sub-category-mobile/sub-category-mobile';
import { VendorLayout } from './vendor/vendor-layout/vendor-layout';
import { VendorRoute } from './vendor.route';
import { AddProduct } from './vendor/vendor-product/add-product/add-product';

export const routes: Routes = [
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'register-vendor', component: RegisterVendor },
    { path: 'products', component: UserProduct },
    { path: 'navbar', component: UserNavbar },
    { path: 'categories', component: CategoryMobile },
    { path: 'add-product', component: AddProduct },
    { path: 'categories/:categoryId/subcategories', component: SubCategoryMobile },
    { path: 'admin', component: AdminLayout, children: AdminRoutes },
    { path: 'vendor', component: VendorLayout, children: VendorRoute }
];

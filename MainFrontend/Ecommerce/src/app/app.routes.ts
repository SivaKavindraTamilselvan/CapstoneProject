import { Routes } from '@angular/router';
import { Login } from './authentication/login/login';
import { Register } from './authentication/register/register';
import { AdminLayout } from './admin-layout/admin-layout';
import { AdminRoutes } from './routes/admin.route';
import { RegisterVendor } from './authentication/register-vendor/register-vendor';
import { UserProduct } from './users/user-product/user-product';
import { UserNavbar } from './users/user-navbar/user-navbar';
import { CategoryMobile } from './users/category-mobile/category-mobile';
import { SubCategoryMobile } from './users/sub-category-mobile/sub-category-mobile';
import { VendorLayout } from './vendor/vendor-layout/vendor-layout';
import { VendorRoute } from './routes/vendor.route';
import { AddProduct } from './vendor/vendor-product/add-product/add-product';
import { UserRoutes } from './routes/user.route';
import { AdminNotificationList } from './notification/admin/admin-notification-list/admin-notification-list';
import { ProfilePage } from './profile-page/profile-page';

export const routes: Routes = [
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'register-vendor', component: RegisterVendor },
    { path: '', redirectTo: 'user/products', pathMatch: 'full' },
    { path: 'categories/:categoryId/subcategories', component: SubCategoryMobile },
    { path: 'admin', component: AdminLayout, children: AdminRoutes },
    { path: 'vendor', component: VendorLayout, children: VendorRoute },
    { path: 'user', component: UserNavbar, children: UserRoutes },
];

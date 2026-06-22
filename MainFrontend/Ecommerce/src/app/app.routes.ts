import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Register } from './register/register';
import { AdminLayout } from './admin-layout/admin-layout';
import { AdminRoutes } from './admin.route';
import { RegisterVendor } from './register-vendor/register-vendor';
import { UserProduct } from './user-product/user-product';
import { UserNavbar } from './user-navbar/user-navbar';
import { CategoryMobile } from './category-mobile/category-mobile';
import { SubCategoryMobile } from './sub-category-mobile/sub-category-mobile';
import { VendorLayout } from './vendor/vendor-layout/vendor-layout';
import { VendorRoute } from './vendor.route';

export const routes: Routes = [
    { path: '', component: Login },
    { path: 'register', component: Register },
    { path: 'register-vendor', component: RegisterVendor },
    { path: 'products', component: UserProduct },
    { path: 'navbar', component: UserNavbar },
    { path: 'categories', component: CategoryMobile },
    { path: 'categories/:categoryId/subcategories', component: SubCategoryMobile },
    { path: 'admin', component: AdminLayout, children: AdminRoutes },
    { path: 'vendor', component: VendorLayout, children: VendorRoute }
];

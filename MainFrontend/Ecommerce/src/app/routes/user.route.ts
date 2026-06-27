import { Routes } from '@angular/router';
import { UserProduct } from '../users/user-product/user-product';
import { CategoryMobile } from '../users/category-mobile/category-mobile';
import { SubCategoryMobile } from '../users/sub-category-mobile/sub-category-mobile';
import { UserHome } from '../users/user-home/user-home';

export const UserRoutes: Routes = [
    {
        path: '',
        component: UserHome
    },
    {
        path: 'products',
        component: UserProduct
    },
    {
        path: 'categories',
        component: CategoryMobile
    },
    {
        path: 'categories/:categoryId/subcategories',
        component: SubCategoryMobile
    },
    {
        path: 'subcategory/:subCategoryId/products',
        component: UserProduct
    }

];
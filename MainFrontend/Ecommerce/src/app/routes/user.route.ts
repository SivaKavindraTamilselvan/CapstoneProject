import { Routes } from '@angular/router';
import { UserProduct } from '../users/user-product/user-product';
import { CategoryMobile } from '../users/category-mobile/category-mobile';
import { SubCategoryMobile } from '../users/sub-category-mobile/sub-category-mobile';
import { UserHome } from '../users/user-home/user-home';
import { UserProductDetails } from '../users/user-product-details/user-product-details';
import { UserCart } from '../users/user-cart/user-cart';
import { UserOrderService } from '../services/user-order.Service';
import { UserAddOrder } from '../users/user-order/user-add-order/user-add-order';
import { OrderSuccess } from '../users/user-order/order-success/order-success';
import { OrderFailure } from '../users/user-order/order-failure/order-failure';
import { UserGetOrder } from '../users/user-order/user-get-order/user-get-order';
import { UserFavorite } from '../users/user-favorite/user-favorite';
import { UserProfilePage } from '../users/user-profile-page/user-profile-page';
import { SharedRoutes } from './shared-route';
import { UserOrderDetails } from '../users/user-order/user-order-details/user-order-details';

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
    },
    {
        path: 'product-details/:id',
        component: UserProductDetails
    },
    {
        path: 'cart',
        component: UserCart
    },
    {
        path: 'favorite',
        component: UserFavorite
    },
    {
        path: 'checkout',
        component: UserAddOrder
    },
    {
        path: 'order-success',
        component: OrderSuccess
    },
    {
        path: 'order-failed',
        component: OrderFailure
    },
    {
        path: 'orders',
        component: UserGetOrder
    },
    {
        path: 'orders/:id',
        component: UserOrderDetails
    },
    {
        path: 'profile',
        component: UserProfilePage
    },

    ...SharedRoutes
];
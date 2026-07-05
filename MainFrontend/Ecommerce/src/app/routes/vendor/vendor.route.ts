import { Routes } from "@angular/router";
import { AddProduct } from "../../vendor/vendor-product/add-product/add-product";
import { ProductList } from "../../vendor/vendor-product/product-list/product-list";
import { AddVariant } from "../../vendor/vendor-variant/add-variant/add-variant";
import { VariantList } from "../../vendor/vendor-variant/variant-list/variant-list";
import { ReviewProduct } from "../../vendor/vendor-product/review-product/review-product";
import { UpdateProduct } from "../../vendor/vendor-product/update-product/update-product";
import { AddAddress } from "../../address/add-address/add-address";
import { AddInventory } from "../../vendor/inventory/add-inventory/add-inventory";
import { InventoryList } from "../../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { DeletedInventory } from "../../vendor/inventory/deleted-inventory/deleted-inventory";
import { VendorProductDetails } from "../../vendor/vendor-product/vendor-product-details/vendor-product-details";
import { UpdateRejectedProduct } from "../../vendor/vendor-product/update-rejected-product/update-rejected-product";
import { ReviewVariant } from "../../vendor/vendor-variant/review-variant/review-variant";
import { UpdateVariant } from "../../vendor/vendor-variant/update-variant/update-variant";
import { UpdateRejectedVariant } from "../../vendor/vendor-variant/update-rejected-variant/update-rejected-variant";
import { VendorOrderList } from "../../vendor/vendor-order/vendor-order-list/vendor-order-list";
import { VendorUpdateOrder } from "../../vendor/vendor-order/vendor-update-order/vendor-update-order";
import { RegisterVendor } from "../../vendor/vendor-user/register-vendor/register-vendor";
import { VendorUserDetails } from "../../vendor/vendor-user/vendor-user-details/vendor-user-details";
import { VendorUserList } from "../../vendor/vendor-user/vendor-user-list/vendor-user-list";
import { ActivateVendorUser } from "../../vendor/vendor-user/activate-vendor-user/activate-vendor-user";
import { DeactiveVendorUser } from "../../vendor/vendor-user/deactive-vendor-user/deactive-vendor-user";
import { VendorInventoryRoute } from "./inventory.route";
import { VendorNotificationList } from "../../notification/vendor-notification-list/vendor-notification-list";
import { VendorDeletedProduct } from "../../vendor/vendor-product/vendor-deleted-product/vendor-deleted-product";
import { VendorVariantDetails } from "../../vendor/vendor-variant/vendor-variant-details/vendor-variant-details";

export const VendorRoute: Routes = [
    {
        path: 'users/register',
        component: RegisterVendor
    },

    {
        path: 'users/list',
        component: VendorUserList
    },
    {
        path: 'users/active',
        component: ActivateVendorUser
    },
    {
        path: 'users/inactive',
        component: DeactiveVendorUser
    },
    {
        path: 'users/:id',
        component: VendorUserDetails
    },
    {
        path: 'products/add',
        component: AddProduct
    },
    {
        path: 'products/list',
        component: ProductList
    },
    {
        path: 'deleted-products/list',
        component: VendorDeletedProduct
    },
    {
        path: 'products/:id/variants/add',
        component: AddVariant
    },
    {
        path: 'products/variants/list',
        component: VariantList
    },
    {
        path: 'products/variants/review',
        component: ReviewVariant
    },
    {
        path: 'products/review',
        component: ReviewProduct
    },
    {
        path: 'products/update-status',
        component: UpdateProduct
    },

    {
        path: 'products/variants/update',
        component: UpdateVariant
    },
    {
        path: 'products/update-rejected',
        component: UpdateRejectedProduct
    },
    {
        path: 'products/variants/update-rejected',
        component: UpdateRejectedVariant
    },
    {
        path: 'products/:id',
        component: VendorProductDetails
    },
     {
        path: 'products/variant/:id',
        component: VendorVariantDetails
    },

    {
        path: 'orders/list',
        component: VendorOrderList
    },
    {
        path: 'orders/confirmed-orders',
        component: VendorUpdateOrder
    },
    {
        path: 'notifications',
        component: VendorNotificationList
    },
    ...VendorInventoryRoute
]
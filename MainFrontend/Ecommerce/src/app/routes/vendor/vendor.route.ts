import { Routes } from "@angular/router";
import { AddProduct } from "../../vendor/vendor-product/add-product/add-product";
import { ProductList } from "../../vendor/vendor-product/product-list/product-list";
import { AddVariant } from "../../vendor/vendor-variant/add-variant/add-variant";
import { VariantList } from "../../vendor/vendor-variant/variant-list/variant-list";
import { AddAddress } from "../../address/add-address/add-address";
import { InventoryList } from "../../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { VendorProductDetails } from "../../vendor/vendor-product/vendor-product-details/vendor-product-details";
import { UpdateVariant } from "../../vendor/vendor-variant/update-variant/update-variant";
import { VendorOrderList } from "../../vendor/vendor-order/vendor-order-list/vendor-order-list";
import { RegisterVendor } from "../../vendor/vendor-user/register-vendor/register-vendor";
import { VendorUserDetails } from "../../vendor/vendor-user/vendor-user-details/vendor-user-details";
import { VendorUserList } from "../../vendor/vendor-user/vendor-user-list/vendor-user-list";
import { VendorInventoryRoute } from "./inventory.route";
import { VendorNotificationList } from "../../notification/vendor-notification-list/vendor-notification-list";
import { VendorVariantDetails } from "../../vendor/vendor-variant/vendor-variant-details/vendor-variant-details";
import { VendorUserRoute } from "./vendor-user.route";
import { VendorProductRoute } from "./vendor-product,route";
import { VendorOrderRoute } from "./vendor-order.route";
import { VendorHomeDashboard } from "../../vendor/vendor-dashboard/vendor-home-dashboard/vendor-home-dashboard";
import { VendorProductVariantRoute } from "./vendor-variant.route";

export const VendorRoute: Routes = [
    {
        path: '',
        component: VendorHomeDashboard
    },

  


    {
        path: 'notifications',
        component: VendorNotificationList
    },
    ...VendorInventoryRoute,
    ...VendorUserRoute,
    ...VendorProductRoute,
    ...VendorOrderRoute,
    ...VendorProductVariantRoute
]
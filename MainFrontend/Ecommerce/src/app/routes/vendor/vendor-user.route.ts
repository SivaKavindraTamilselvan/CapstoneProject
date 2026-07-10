import { Routes } from "@angular/router";
import { InventoryList } from "../../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { AddWarehouse } from "../../vendor/vendor-warehouse/add-warehouse/add-warehouse";
import { VendorWarehouseList } from "../../vendor/vendor-warehouse/vendor-warehouse-list/vendor-warehouse-list";
import { VendorUserList } from "../../vendor/vendor-user/vendor-user-list/vendor-user-list";
import { VendorUserDetails } from "../../vendor/vendor-user/vendor-user-details/vendor-user-details";
import { RegisterVendor } from "../../vendor/vendor-user/register-vendor/register-vendor";

export const VendorUserRoute: Routes = [
    {
        path: 'users/register',
        component: RegisterVendor
    },

    {
        path: 'users/list',
        component: VendorUserList,
        data: { status: null, title: 'Vendor User List' }
    },
    {
        path: 'users/active',
        component: VendorUserList,
        data: { status: true, title: 'Active Vendor User' }
    },
    {
        path: 'users/inactive',
        component: VendorUserList,
         data: { status: false, title: 'Inactive Vendor User' }
    },
    {
        path: 'users/:id',
        component: VendorUserDetails
    },
]
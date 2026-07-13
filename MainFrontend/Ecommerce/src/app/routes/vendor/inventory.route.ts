import { Routes } from "@angular/router";
import { InventoryList } from "../../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { AddWarehouse } from "../../vendor/vendor-warehouse/add-warehouse/add-warehouse";
import { VendorWarehouseList } from "../../vendor/vendor-warehouse/vendor-warehouse-list/vendor-warehouse-list";
import { GetWarehouseAddress } from "../../vendor/vendor-warehouse/get-warehouse-address/get-warehouse-address";

export const VendorInventoryRoute: Routes = [
    {
        path: 'warehouses/add',
        component: AddWarehouse
    },
    {
        path: 'warehouses',
        component: VendorWarehouseList,
        data: { status: true, title: 'Warehouse List' }
    },
    {
        path: 'warehouses/:id',
        component: GetWarehouseAddress
    },
    {
        path: 'deleted-warehouses',
        component: VendorWarehouseList,
        data: { status: false, title: 'Deleted Warehouse List' }
    },
    {
        path: 'inventory/add',
        component: VendorWarehouseList,
          data: { status: true, title: 'Add Inventory' }
    },
    {
        path: 'inventory',
        component: InventoryList,
        data: { status: true, title: 'Inventory List' }
    },
    {
        path: 'deleted-inventory',
        component: InventoryList,
        data: { status: false, title: 'Deleted Inventory List' }
    },
    {
        path: 'inventory-details/:id',
        component: VendorInventoryDetails
    },
]
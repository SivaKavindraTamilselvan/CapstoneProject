import { Routes } from "@angular/router";
import { AddInventory } from "../../vendor/inventory/add-inventory/add-inventory";
import { InventoryList } from "../../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { DeletedInventory } from "../../vendor/inventory/deleted-inventory/deleted-inventory";
import { AddWarehouse } from "../../vendor/vendor-warehouse/add-warehouse/add-warehouse";
import { VendorWarehouseList } from "../../vendor/vendor-warehouse/vendor-warehouse-list/vendor-warehouse-list";
import { VendorDeletedWarehouse } from "../../vendor/vendor-warehouse/vendor-deleted-warehouse/vendor-deleted-warehouse";

export const VendorInventoryRoute: Routes = [
    {
        path: 'warehouses/add',
        component: AddWarehouse
    },
    {
        path: 'warehouses',
        component: VendorWarehouseList
    },
    {
        path: 'deleted-warehouses',
        component: VendorDeletedWarehouse
    },
    {
        path: 'inventory/add',
        component: AddInventory
    },
    {
        path: 'inventory',
        component: InventoryList
    },
    {
        path: 'deleted-inventory',
        component: DeletedInventory
    },
    {
        path: 'inventory-details/:id',
        component: VendorInventoryDetails
    },
]
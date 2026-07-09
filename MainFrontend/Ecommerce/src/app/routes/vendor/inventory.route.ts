import { Routes } from "@angular/router";
import { AddInventory } from "../../vendor/inventory/add-inventory/add-inventory";
import { InventoryList } from "../../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { DeletedInventory } from "../../vendor/inventory/deleted-inventory/deleted-inventory";
import { AddWarehouse } from "../../vendor/vendor-warehouse/add-warehouse/add-warehouse";
import { VendorWarehouseList } from "../../vendor/vendor-warehouse/vendor-warehouse-list/vendor-warehouse-list";

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
        component: DeletedInventory,
        data: { status: false, title: 'Deleted Inventory List' }
    },
    {
        path: 'inventory-details/:id',
        component: VendorInventoryDetails
    },
]
import { Routes } from "@angular/router";
import { AddInventory } from "../../vendor/inventory/add-inventory/add-inventory";
import { InventoryList } from "../../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { DeletedInventory } from "../../vendor/inventory/deleted-inventory/deleted-inventory";
import { AddWarehouse } from "../../vendor/vendor-warehouse/add-warehouse/add-warehouse";
import { VendorWarehouseList } from "../../vendor/vendor-warehouse/vendor-warehouse-list/vendor-warehouse-list";
import { VendorDeletedWarehouse } from "../../vendor/vendor-warehouse/vendor-deleted-warehouse/vendor-deleted-warehouse";
import { AddProduct } from "../../vendor/vendor-product/add-product/add-product";
import { ProductList } from "../../vendor/vendor-product/product-list/product-list";
import { VendorProductDetails } from "../../vendor/vendor-product/vendor-product-details/vendor-product-details";

export const VendorProductRoute: Routes = [

    {
        path: 'products/add',
        component: AddProduct
    },
    {
        path: 'products/list',
        component: ProductList,
        data: { status: null, deleted: false, title: 'Product List' }
    },
    {
        path: 'deleted-products/list',
        component: ProductList,
        data: { status: null, deleted: true, title: 'Deleted Product List' }
    },
    {
        path: 'products/review',
        component: ProductList,
        data: { status: 1, deleted: false, title: 'Review Product List' }
    },
    {
        path: 'products/update-status',
        component: ProductList,
        data: { status: null, deleted: false, title: 'Update Product' }
    },
    {
        path: 'products/update-rejected',
        component: ProductList,
        data: { status: 6, deleted: false, title: 'Product List' }
    }, {
        path: 'products/:id',
        component: VendorProductDetails
    },
]
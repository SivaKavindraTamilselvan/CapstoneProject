import { Routes } from "@angular/router";
import { AddProduct } from "../vendor/vendor-product/add-product/add-product"; 
import { ProductList } from "../vendor/vendor-product/product-list/product-list";
import { AddVariant } from "../vendor/vendor-variant/add-variant/add-variant";
import { VariantList } from "../vendor/vendor-variant/variant-list/variant-list";
import { ReviewProduct } from "../vendor/vendor-product/review-product/review-product";
import { UpdateProduct } from "../vendor/vendor-product/update-product/update-product";
import { AddAddress } from "../address/add-address/add-address";
import { GetVendorAddress } from "../address/get-vendor-address/get-vendor-address";
import { AddInventory } from "../vendor/inventory/add-inventory/add-inventory";
import { InventoryList } from "../vendor/inventory/inventory-list/inventory-list";
import { VendorInventoryDetails } from "../vendor/inventory/vendor-inventory-details/vendor-inventory-details";
import { DeletedInventory } from "../vendor/inventory/deleted-inventory/deleted-inventory";
import { VendorProductDetails } from "../vendor/vendor-product/vendor-product-details/vendor-product-details";
import { UpdateRejectedProduct } from "../vendor/vendor-product/update-rejected-product/update-rejected-product";

export const VendorRoute: Routes = [
    {
        path: 'products/add',
        component: AddProduct
    },
    {
        path: 'products/list',
        component: ProductList
    },
    {
        path: 'products/variants/add',
        component: AddVariant
    },
    {
        path: 'products/variants/list',
        component: VariantList
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
        path: 'products/update-rejected',
        component: UpdateRejectedProduct
    },
    {
        path: 'warehouses/add',
        component: AddAddress
    },
    {
        path: 'warehouses',
        component: GetVendorAddress
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
    {
        path: 'products/:id',
        component: VendorProductDetails
    },
]
import { Routes } from "@angular/router";
import { AddProduct } from "./vendor/vendor-product/add-product/add-product";
import { ProductList } from "./vendor/vendor-product/product-list/product-list";

export const VendorRoute: Routes = [
    {
        path: 'products/add',
        component: AddProduct
    },
    {
        path: 'products/list',
        component: ProductList
    }
]
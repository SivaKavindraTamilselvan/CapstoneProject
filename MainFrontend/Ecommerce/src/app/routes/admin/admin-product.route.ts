import { Routes } from "@angular/router";
import { RegisterAdmin } from "../../admin/admin-user/register-admin/register-admin";
import { AdminList } from "../../admin/admin-user/admin-list/admin-list";
import { AdminUserDetail } from "../../admin/admin-user/admin-user-detail/admin-user-detail";
import { AdminProduct } from "../../admin/admin-product/admin-product/admin-product";
import { AdminDetailProduct } from "../../admin/admin-product/admin-detail-product/admin-detail-product";
import { ReviewVariant } from "../../admin/admin-product/review-variant/review-variant";
import { VariantList } from "../../admin/admin-product/variant-list/variant-list";
import { VendorVariantDetails } from "../../vendor/vendor-variant/vendor-variant-details/vendor-variant-details";
import { AdminVariantDetails } from "../../admin/admin-product/admin-variant-details/admin-variant-details";


export const AdminProductRoutes: Routes = [
    {
        path: 'products/list',
        component: AdminProduct,
        data: { status: null, deleted: false, title: 'Product List' }
    },
    {
        path: 'products/delete',
        component: AdminProduct,
        data: { status: null, deleted: true, title: 'Deleted Product' }
    },
    {
        path: 'products/review',
        component: AdminProduct,
        data: { status: 2, deleted: false, title: 'Pending Review Product' }
    },
    {
        path: 'product-details/:id',
        component: AdminDetailProduct,
    },
    {
        path: 'product-variant/review',
        component: VariantList,
        data: { status: 2, deleted: false, title: 'Pending Review Product' }
    },
    {
        path: 'product-variant/list',
        component: VariantList,
        data: { status: null, deleted: false, title: 'Product List' }
    },
     {
        path: 'products-variant/delete',
        component: VariantList,
       data: { status: null, deleted: true, title: 'Deleted Product' }
    },
    {
        path: 'product-variant-details/:id',
        component: AdminVariantDetails
    },
];
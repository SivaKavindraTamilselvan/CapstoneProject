import { Routes } from "@angular/router";
import { RegisterAdmin } from "../../admin/admin-user/register-admin/register-admin";
import { AdminList } from "../../admin/admin-user/admin-list/admin-list";
import { AdminUserDetail } from "../../admin/admin-user/admin-user-detail/admin-user-detail";
import { AdminProduct } from "../../admin/admin-product/admin-product/admin-product";
import { AdminDetailProduct } from "../../admin/admin-product/admin-detail-product/admin-detail-product";
import { ReviewVariant } from "../../admin/admin-product/review-variant/review-variant";
import { VariantList } from "../../admin/admin-product/variant-list/variant-list";


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
        data: { status: 1, deleted: false, title: 'Pending Review Product' }
    },
    {
        path: 'product-details/:id',
        component: AdminDetailProduct,
    },

    {
        path: 'product-variant/review',
        component: ReviewVariant
    },
    {
        path: 'product-variant/list',
        component: VariantList
    },


];
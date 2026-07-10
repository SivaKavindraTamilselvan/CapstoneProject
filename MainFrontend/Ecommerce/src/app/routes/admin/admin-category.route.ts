import { Routes } from "@angular/router";
import { RegisterAdmin } from "../../admin/admin-user/register-admin/register-admin";
import { AdminList } from "../../admin/admin-user/admin-list/admin-list";
import { AdminUserDetail } from "../../admin/admin-user/admin-user-detail/admin-user-detail";
import { AdminProduct } from "../../admin/admin-product/admin-product/admin-product";
import { AdminDetailProduct } from "../../admin/admin-product/admin-detail-product/admin-detail-product";
import { ReviewVariant } from "../../admin/admin-product/review-variant/review-variant";
import { VariantList } from "../../admin/admin-product/variant-list/variant-list";
import { CategoryList } from "../../admin/admin-category/category-list/category-list";
import { Subcategorylist } from "../../admin/admin-subcategory/subcategorylist/subcategorylist";


export const AdminCategoryRoutes: Routes = [
    {
        path: 'product-category/list',
        component: CategoryList,
        data: { status: null, title: 'Product Category List' }
    },
    {
        path: 'product-category/list?status=active',
        component: CategoryList,
        data: { status: true, title: 'Active Category List' }
    },
    {
        path: 'product-category/list?status=inactive',
        component: CategoryList,
        data: { status: false, title: 'Inactive Category List' }
    },
    {
        path: 'product-sub-category/list',
        component: Subcategorylist,
        data: { status: null, title: 'Product Sub Category List' }
    },
    {
        path: 'product-sub-category/list?status=active',
        component: Subcategorylist,
        data: { status: true, title: 'Active Sub Category List' }
    },
    {
        path: 'product-sub-category/list?status=inactive',
        component: Subcategorylist,
        data: { status: false, title: 'Inactive Sub Category List' }
    },

];
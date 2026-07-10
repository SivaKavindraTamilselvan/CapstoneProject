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
import { AttributeList } from "../../admin/admin-attribute/attribute-list/attribute-list";
import { MappedAttributeList } from "../../admin/admin-mapped-attribute/mapped-attribute-list/mapped-attribute-list";
import { GetAdminOrders } from "../../admin/admin-orders/get-admin-orders/get-admin-orders";
import { AdminCancelledOrder } from "../../admin/admin-orders/admin-cancelled-order/admin-cancelled-order";


export const AdminOrderRoutes: Routes = [
    {
        path: 'orders/list',
        component: GetAdminOrders,
        data: { status: null, title: 'Inactive Mapped Attribute List' }
    },
    {
        path: 'orders/confirmed-orders',
        component: GetAdminOrders,
        data: { status: 2, title: 'Inactive Mapped Attribute List' }
    },
    {
        path: 'orders/cancelled-orders',
        component: AdminCancelledOrder,
    },

];
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
import { AdminCancelDetailOrder } from "../../admin/admin-orders/admin-cancel-detail-order/admin-cancel-detail-order";
import { AdminReturDetails } from "../../admin/admin-orders/admin-retur-details/admin-retur-details";
import { AdminReturnOrders } from "../../admin/admin-orders/admin-return-orders/admin-return-orders";


export const AdminOrderRoutes: Routes = [
    {
        path: 'orders/list',
        component: GetAdminOrders,
        data: { status: null, title: 'Order List' }
    },
    {
        path: 'orders/confirmed-orders',
        component: GetAdminOrders,
        data: { status: 2, title: 'OnGoing Order List' }
    },
    {
        path: 'orders/cancelled-orders',
        component: AdminCancelledOrder,
    },
    {
        path: 'orders/cancelled-order/:id',
        component: AdminCancelDetailOrder,
    },
    {
        path: 'orders/return-order/:id',
        component: AdminReturDetails,
    },
    {
        path: 'returns/list',
        component: AdminReturnOrders,
        data: { status: null, ongoing: false, title: 'Return Order List' }
    },
    {
        path: 'returns/ongoing',
        component: AdminReturnOrders,
        data: { status: null, ongoing: true, title: 'OnGoing Return List' }
    },
    {
        path: 'returns/dispute',
        component: AdminReturnOrders,
        data: { status: 11, ongoing: false, title: 'Dispute Return List' }
    },

];
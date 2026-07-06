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
import { AddCoupon } from "../../admin/admin-coupon/add-coupon/add-coupon";
import { CouponList } from "../../admin/admin-coupon/coupon-list/coupon-list";
import { CouponDetail } from "../../admin/admin-coupon/coupon-detail/coupon-detail";


export const AdminCouponRoutes: Routes = [
    {
        path: 'coupon/add',
        component: AddCoupon
    },
    {
        path: 'coupon/list',
        component: CouponList,
        data: { status: null, title: 'Coupon List' }
    },
    {
        path: 'coupons/:id',
        component: CouponDetail
    },
    {
        path: 'coupon/list?status=active',
        component: CouponList,
        data: { status: true, title: 'Active Coupons' }
    },
    {
        path: 'coupon/list?status=inactive',
        component: CouponList,
        data: { status: false, title: 'Inactive Coupons' }
    },
];
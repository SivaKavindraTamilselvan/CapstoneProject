import { Routes } from '@angular/router';
import { RegisterAdmin } from '../admin/admin-user/register-admin/register-admin';
import { AdminList } from '../admin/admin-user/admin-list/admin-list';
import { AdminProduct } from '../admin/admin-product/admin-product/admin-product';
import { ActivateAdmin } from '../admin/admin-user/activate-admin/activate-admin';
import { DeactivateAdmin } from '../admin/admin-user/deactivate-admin/deactivate-admin';
import { ReviewVendor } from '../admin/admin-vendor/review-vendor/review-vendor';
import { VendorList } from '../admin/admin-vendor/vendor-list/vendor-list';
import { DeleteVendor } from '../admin/admin-vendor/delete-vendor/delete-vendor';
import { ReviewProduct } from '../admin/admin-product/review-product/review-product';
import { DeleteProduct } from '../admin/admin-product/delete-product/delete-product';
import { CategoryList } from '../admin/admin-category/category-list/category-list';
import { ActiveCategory } from '../admin/admin-category/active-category/active-category';
import { InactiveCategory } from '../admin/admin-category/inactive-category/inactive-category';
import { Subcategorylist } from '../admin/admin-subcategory/subcategorylist/subcategorylist';
import { InactiveSubCategory } from '../admin/admin-subcategory/inactive-sub-category/inactive-sub-category';
import { ActiveSubCategory } from '../admin/admin-subcategory/active-sub-category/active-sub-category';
import { AttributeList } from '../admin/admin-attribute/attribute-list/attribute-list';
import { ActiveAttribute } from '../admin/admin-attribute/active-attribute/active-attribute';
import { InactiveAttribute } from '../admin/admin-attribute/inactive-attribute/inactive-attribute';
import { MappedAttributeList } from '../admin/admin-mapped-attribute/mapped-attribute-list/mapped-attribute-list';
import { ActiveMappedAttribute } from '../admin/admin-mapped-attribute/active-mapped-attribute/active-mapped-attribute';
import { InactiveMappedAttribute } from '../admin/admin-mapped-attribute/inactive-mapped-attribute/inactive-mapped-attribute';
import { AdminDetailProduct } from '../admin/admin-product/admin-detail-product/admin-detail-product';
import { AdminUserDetail } from '../admin/admin-user/admin-user-detail/admin-user-detail';
import { VendorDetails } from '../admin/admin-vendor/vendor-details/vendor-details';
import { AddCoupon } from '../admin/admin-coupon/add-coupon/add-coupon';
import { GetAdminOrders } from '../admin/admin-orders/get-admin-orders/get-admin-orders';
import { AdminConfirmedOrders } from '../admin/admin-orders/admin-confirmed-orders/admin-confirmed-orders';
import { ShipmentList } from '../admin/admin-shipment/shipment-list/shipment-list';
import { UpdateShipment } from '../admin/admin-shipment/update-shipment/update-shipment';
import { ReviewVariant } from '../admin/admin-product/review-variant/review-variant';
import { VariantList } from '../admin/admin-product/variant-list/variant-list';
import { CouponList } from '../admin/admin-coupon/coupon-list/coupon-list';
import { CouponDetail } from '../admin/admin-coupon/coupon-detail/coupon-detail';
import { ActiveCoupon } from '../admin/admin-coupon/active-coupon/active-coupon';
import { DeactiveCoupon } from '../admin/admin-coupon/deactive-coupon/deactive-coupon';
import { AdminNotificationList } from '../notification/admin/admin-notification-list/admin-notification-list';
import { AdminProfilePage } from '../admin/admin-profile-page/admin-profile-page';
import { AdminOrderDetails } from '../admin/admin-orders/admin-order-details/admin-order-details';
import { AdminShipmentDetails } from '../admin/admin-shipment/admin-shipment-details/admin-shipment-details';
import { AdminUserRoutes } from './admin/admin-user.route';

export const AdminRoutes: Routes = [
    {
        path: 'users/register',
        component: RegisterAdmin
    },
    {
        path: 'users/list',
        component: AdminList,
        data: { status: null, title: 'Admin List' }
    },
    {
        path: 'users/activate',
        component: AdminList,
        data: { status: true, title: 'Active Admins' }
    },
    {
        path: 'users/deactivate',
        component: AdminList,
        data: { status: false, title: 'Inactive Admins' }
    },
    {
        path: 'users/:id',
        component: AdminUserDetail
    },
    {
        path: 'vendors/review',
        component: ReviewVendor
    },
    {
        path: 'vendors/list',
        component: VendorList
    },
    {
        path: 'vendors/delete',
        component: DeleteVendor
    },
    {
        path: 'vendors/:id',
        component: VendorDetails
    },
    {
        path: 'products/list',
        component: AdminProduct
    },
    {
        path: 'product-details/:id',
        component: AdminDetailProduct
    },
    {
        path: 'products/review',
        component: ReviewProduct
    },
     {
        path: 'product-variant/review',
        component: ReviewVariant
    },
    {
        path: 'product-variant/list',
        component: VariantList
    },
    {
        path: 'products/delete',
        component: DeleteProduct
    },
    {
        path: 'product-category/list',
        component: CategoryList
    },
    {
        path: 'product-category/list?status=active',
        component: ActiveCategory
    },
    {
        path: 'product-category/list?status=inactive',
        component: InactiveCategory
    },
    {
        path: 'product-sub-category/list',
        component: Subcategorylist
    },
    {
        path: 'product-sub-category/list?status=active',
        component: ActiveSubCategory
    },
    {
        path: 'product-sub-category/list?status=inactive',
        component: InactiveSubCategory
    },
    {
        path: 'product-attribute/list',
        component: AttributeList
    },
    {
        path: 'product-attribute/list?status=active',
        component: ActiveAttribute
    },
    {
        path: 'product-attribute/list?status=inactive',
        component: InactiveAttribute
    },
    {
        path: 'attribute-mapping/list',
        component: MappedAttributeList
    },
    {
        path: 'attribute-mapping/list?status=active',
        component: ActiveMappedAttribute
    },
    {
        path: 'attribute-mapping/list?status=inactive',
        component: InactiveMappedAttribute
    },
    {
        path: 'coupon/add',
        component: AddCoupon
    },
    {
        path: 'coupon/list',
        component: CouponList
    },
    {
        path: 'coupons/:id',
        component: CouponDetail
    },
    {
        path: 'coupon/list?status=active',
        component: ActiveCoupon
    },
     {
        path: 'coupon/list?status=inactive',
        component: DeactiveCoupon
    },
    {
        path: 'orders/list',
        component: GetAdminOrders
    },
    {
        path: 'orders/confirmed-orders',
        component: AdminConfirmedOrders
    },
    {
        path: 'shipments/list',
        component: ShipmentList
    },
    {
        path: 'shipments/update',
        component: UpdateShipment
    },
    {
        path: 'notifications',
        component: AdminNotificationList
    },
    {
        path: 'profile',
        component: AdminProfilePage
    },
    {
        path: 'order/:id',
        component: AdminOrderDetails
    },
     {
        path: 'shipment-details/:id',
        component: AdminShipmentDetails
    },
    ...AdminUserRoutes,

];
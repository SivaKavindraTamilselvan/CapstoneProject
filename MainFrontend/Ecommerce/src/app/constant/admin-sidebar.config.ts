export interface SidebarChild {
  label: string;
  route: string;
}

export interface SidebarItem {
  key: string;
  label: string;
  roles: string[];
  children?: SidebarChild[];
}

export const SIDEBAR_MENU: SidebarItem[] = [
  {
    key: 'admin-user',
    label: 'Admin User',
    roles: ['super-admin'],
    children: [
      { label: 'Register Admin', route: '/admin/users/register' },
      { label: 'Admin List', route: '/admin/users/list' },
      { label: 'Active Admin', route: '/admin/users/activate' },
      { label: 'Inactive Admin', route: '/admin/users/deactivate' },
    ]
  },
  {
    key: 'vendor',
    label: 'Vendor',
    roles: ['super-admin'],
    children: [
      { label: 'Review Vendor', route: '/admin/vendors/review' },
      { label: 'Vendor List', route: '/admin/vendors/list' },
      { label: 'Deleted Vendor', route: '/admin/vendors/delete' },
    ]
  },
  {
    key: 'review-vendor',
    label: 'Review Vendor',
    roles: ['vendor-admin'],
  },
  {
    key: 'active-vendor',
    label: 'Active Vendor',
    roles: ['vendor-admin'],
  },
  {
    key: 'vendor-list',
    label: 'Vendor List',
    roles: ['vendor-admin'],
  },
  {
    key: 'delete-vendor',
    label: 'Delete Vendor',
    roles: ['vendor-admin'],
  },
  {
    key: 'products',
    label: 'Products',
    roles: ['super-admin', 'product-admin'],
    children: [
      { label: 'Review Product', route: '/admin/products/review' },
      { label: 'Product List', route: '/admin/products/list' },
      { label: 'Delete Product', route: '/admin/products/delete' },
    ]
  },
  {
    key: 'products-variant',
    label: 'Product Variant',
    roles: ['super-admin', 'product-admin'],
    children: [
      { label: 'Review Product Variant', route: '/admin/product-variant/review' },
      { label: 'Product Variant List', route: '/admin/product-variant/list' },
      { label: 'Delete Product', route: '/admin/products-variant/delete' },
    ]
  },
  {
    key: 'product-category',
    label: 'Product Category',
    roles: ['super-admin', 'product-admin'],
    children: [
      { label: 'Category List', route: '/admin/product-category/list' },
      { label: 'Active Category', route: '/admin/product-category/list?status=active' },
      { label: 'Inactive Category', route: '/admin/product-category/list?status=inactive' },
    ]
  },
  {
    key: 'product-sub-category',
    label: 'Product Sub Category',
    roles: ['super-admin', 'product-admin'],
    children: [
      { label: 'Sub Category List', route: '/admin/product-sub-category/list' },
      { label: 'Active Sub Category', route: '/admin/product-sub-category/list?status=active' },
      { label: 'Inactive Sub Category', route: '/admin/product-sub-category/list?status=inactive' },
    ]
  },
  {
    key: 'product-attribute',
    label: 'Product Attribute',
    roles: ['super-admin', 'product-admin'],
    children: [
      { label: 'Attribute List', route: '/admin/product-attribute/list' },
      { label: 'Active Attributes', route: '/admin/product-attribute/list?status=active' },
      { label: 'Inactive Attributes', route: '/admin/product-attribute/list?status=inactive' },
    ]
  },
  {
    key: 'mapped-product-attribute',
    label: 'Map Attribute',
    roles: ['super-admin', 'product-admin'],
    children: [
      { label: 'Mapped Attribute List', route: '/admin/attribute-mapping/list' },
      { label: 'Active Mapped Attributes', route: '/admin/attribute-mapping/list?status=active' },
      { label: 'Inactive Mapped Attributes', route: '/admin/attribute-mapping/list?status=inactive' },
    ]
  },
  {
    key: 'coupon',
    label: 'Coupons',
    roles: ['super-admin', 'product-admin'],
    children: [
      { label: 'Add Coupon', route: '/admin/coupon/add' },
      { label: 'Active Coupons', route: '/admin/coupon/list?status=active' },
      { label: 'Inactive Coupons', route: '/admin/coupon/list?status=inactive' },
      { label: 'All Coupons', route: '/admin/coupon/list' }
    ]
  },
  {
    key: 'orders',
    label: 'Orders',
    roles: ['super-admin', 'order-admin', 'vendor-admin'],
    children: [
      { label: 'OnGoing Orders', route: '/admin/orders/confirmed-orders' },
      { label: 'All Orders', route: '/admin/orders/list' }
    ]
  },
  {
  key: 'shipments',
  label: 'Shipments',
  roles: ['super-admin', 'order-admin', 'vendor-admin'],
  children: [
    { label: 'Shipment List', route: '/admin/shipments/list' },
    { label: 'Update Shipment', route: '/admin/shipments/update' },
    { label: 'Shipment Lost', route: '/admin/shipments/lost' }
  ]
}
];
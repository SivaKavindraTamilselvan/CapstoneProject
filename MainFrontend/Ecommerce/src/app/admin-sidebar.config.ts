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
      { label: 'Activate Admin', route: '/admin/users/activate' },
      { label: 'Deactivate Admin', route: '/admin/users/deactivate' },
    ]
  },
  {
    key: 'vendor',
    label: 'Vendor',
    roles: ['super-admin'],
    children: [
      { label: 'Review Vendor', route: '/admin/vendors/review' },
      { label: 'Active Vendors', route: '/admin/vendors/active' },
      { label: 'Vendor List', route: '/admin/vendors/list' },
      { label: 'Delete Vendor', route: '/admin/vendors/delete' },
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
      { label: 'Available Product', route: '/admin/products/available' },
      { label: 'Active Product', route: '/admin/products/active' },
      { label: 'Archived Product', route: '/admin/products/archived' },
      { label: 'Product List', route: '/admin/products/list' },
      { label: 'Delete Product', route: '/admin/products/delete' },
    ]
  },
];
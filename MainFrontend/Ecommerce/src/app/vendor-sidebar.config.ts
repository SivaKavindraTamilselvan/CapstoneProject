export interface VendorSidebarChild {
    label: string;
    route: string;
    roles: string[];
}

export interface VendorSidebarItem {
    key: string;
    label: string;
    roles: string[];
    children?: VendorSidebarChild[];
}

export const VENDOR_SIDEBAR_MENU: VendorSidebarItem[] = [
    {
        key: 'vendor-user',
        label: 'Vendor User',
        roles: ['owner'],
        children: [
            { label: 'Register Vendor User', route: '/vendor/users/register', roles: ['owner'] },
            { label: 'Vendor User List', route: '/vendor/users/list', roles: ['owner'] },
            { label: 'Active Users', route: '/vendor/users/active', roles: ['owner'] },
            { label: 'Inactive Users', route: '/vendor/users/inactive', roles: ['owner'] },
        ]
    },
    {
        key: 'product',
        label: 'Product',
        roles: ['owner', 'product-manager'],
        children: [
            { label: 'Add Product', route: '/vendor/products/add', roles: ['owner', 'product-manager'] },
            { label: 'Product List', route: '/vendor/products/list', roles: ['owner', 'product-manager'] },
        ]
    },
    {
        key: 'product-variant',
        label: 'Product Variant',
        roles: ['owner', 'product-manager'],
        children: [
            { label: 'Add Product Variant', route: '/vendor/products/variants/add', roles: ['owner', 'product-manager'] },
            { label: 'Product Variant List', route: '/vendor/products/variants/list', roles: ['owner', 'product-manager'] },
        ]
    },
    {
        key: 'review-product',
        label: 'Review Product',
        roles: ['owner'],
        children: [
            { label: 'Review Products', route: '/vendor/products/review', roles: ['owner'] },
            { label: 'Update Product Status', route: '/vendor/products/update-status', roles: ['owner'] },
            { label: 'Update Rejected Product', route: '/vendor/products/update-rejected', roles: ['owner'] },
        ]
    },
    {
        key: 'review-product-variant',
        label: 'Review Product Variant',
        roles: ['owner'],
        children: [
            { label: 'Review Product Variant', route: '/vendor/products/variants/review', roles: ['owner'] },
            { label: 'Update Product Variant', route: '/vendor/products/variants/update', roles: ['owner'] },
            { label: 'Update Rejected Variant', route: '/vendor/products/variants/update-rejected', roles: ['owner'] },
        ]
    },
    {
        key: 'inventory',
        label: 'Inventory',
        roles: ['owner'],
        children: [
            { label: 'Inventory List', route: '/vendor/inventory', roles: ['owner'] },
            { label: 'Deleted Inventory List', route: '/vendor/deleted-inventory/', roles: ['owner'] },
            { label: 'Add Inventory', route: '/vendor/inventory/add', roles: ['owner'] },
            { label: 'Warehouse List', route: '/vendor/warehouses', roles: ['owner'] },
            { label: 'Add Warehouse', route: '/vendor/warehouses/add', roles: ['owner'] }
        ]
    }
];
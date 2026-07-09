import { Routes } from "@angular/router";

import { VendorOrderList } from "../../vendor/vendor-order/vendor-order-list/vendor-order-list";
import { VendorReturnOrder } from "../../vendor/vendor-return/vendor-return-order/vendor-return-order";

export const VendorOrderRoute: Routes = [
    {
        path: 'orders/list',
        component: VendorOrderList,
        data: { status: null, order: null, title: 'Update Product' }
    },
    
    {
        path: 'orders/confirmed-orders',
        component: VendorOrderList,
        data: { status: 1, order: 2, title: 'Update Product' }
    },
    {
        path: 'orders/return-orders',
        component: VendorReturnOrder,
        data: { status: 1, order: 2, title: 'Update Product' }
    },
]
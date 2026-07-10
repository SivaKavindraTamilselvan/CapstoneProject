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
        path: 'returns/pending',
        component: VendorReturnOrder,
        data: { status: 1, title: 'Pending Return List' }
    },
    {
        path: 'returns/inspection',
        component: VendorReturnOrder,
        data: { status: 6, title: 'Product Inspection List' }
    },
    {
        path: 'returns/disputes',
        component: VendorReturnOrder,
        data: { status: 10, order: 2, title: 'Disputed Returns' }
    },
    {
        path: 'returns',
        component: VendorReturnOrder,
        data: { status: null,  title: 'Return List' }
    },
]
export class VendorOrderFilter {
    constructor(
        public pageNumber: number = 1,
        public pageSize: number = 10,
        public orderNumber: string = '',
        public fromDate: string = '',
        public toDate: string = '',
        public orderStatusId: number | null = null,
        public orderItemStatusId: number | null = null,
        public minAmount: number | null = null,
        public maxAmount: number | null = null,
        public userId: number | null = null,
    ) { }
}

export class RequestVendorReturnFilter {
    constructor(
        public returnStatusId: number | null = null,
        public returnReasonId: number | null = null,
        public orderItemId: number | null = null,
        public orderId: number | null = null,
        public fromDate: string | null = null,
        public toDate: string | null = null,
        public pageNumber: number = 1,
        public pageSize: number = 10,
    ) { }
}
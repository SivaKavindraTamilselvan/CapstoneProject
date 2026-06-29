export class AdminOrderFilter {
    constructor(
        public pageNumber: number = 1,
        public pageSize: number = 10,
        public orderNumber?: string,
        public fromDate?: string, // ISO string (date-time)
        public toDate?: string,   // ISO string (date-time)
        public orderStatusId?: number,
        public minAmount?: number,
        public maxAmount?: number,
        public userId?: number,
        public vendorId?: number,
    ) { }
}
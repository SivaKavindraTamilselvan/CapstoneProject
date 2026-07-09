export class ReturnListModel {
    constructor(
        public returnId: number = 0,
        public productImageUrl: string = '',
        public productName: string = '',
        public sku: string = '',
        public vendorName: string = '',
        public returnQuantity: number | null = null,
        public returnAmount: number | null = null,
        public returnReason: string = '',
        public returnStatus: string = '',
        public inventoryCity: string = '',
        public deliveryCity: string = '',
        public requestedDate: string = '',
        public reviewedDate: string = ''
    ) { }
}
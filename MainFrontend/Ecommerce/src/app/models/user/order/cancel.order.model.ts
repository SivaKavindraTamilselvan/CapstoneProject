export class CancelOrderModel {
    constructor(
        public cancelReasonId: number = 0,
        public orderItemId: number = 0,
        public cancelStatusId: number = 1,
        public additionalReason: string = '',
        public cancelQuantity: number = 0,
    ) { }
}

export class CancelSummaryModel {
  constructor(
    public cancelId: number = 0,
    public orderItemId: number = 0,
    public orderId: number = 0,
    public orderNumber: string = '',
    public productName: string = '',
    public sku: string = '',
    public vendorName: string = '',
    public orderedQuantity: number = 0,
    public cancelQuantity: number = 0,
    public unitPrice: number = 0,
    public discount: number = 0,
    public convenienceFee: number = 0,
    public cancelAmount: number = 0,
    public cancelReason: string = '',
    public cancelStatus: string = '',
    public additionalReason: string | null = null,
    public cancelledDate: string = '',
    public deliveryCity: string = '',
    public deliveryAddress: string = '',
    public deliveryPincode: string = ''
  ) {}
}

export class RequestAdminCancelFilter {
  constructor(
    public cancelStatusId: number | null = null,
    public cancelReasonId: number | null = null,
    public vendorId: number | null = null,
    public orderId: number | null = null,
    public orderItemId: number | null = null,
    public productVariantId: number | null = null,
    public fromDate: string | null = null,
    public toDate: string | null = null,
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}

export class RequestVendorCancelFilter {
  constructor(
    public cancelStatusId: number | null = null,
    public cancelReasonId: number | null = null,
    public orderId: number | null = null,
    public productVariantId: number | null = null,
    public orderItemId: number | null = null,
    public fromDate: string | null = null,
    public toDate: string | null = null,
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}
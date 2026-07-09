export class AddReturnModel {
  constructor(
    public returnReasonId: number = 0,
    public orderItemId: number = 0,
    public additionalReason: string = '',
    public returnQuantity: number = 1
  ) {}
}

export class ReturnSummaryModel {
  constructor(
    public returnId: number = 0,
    public productImageUrl: string = '',
    public productName: string = '',
    public sku: string = '',
    public vendorName: string = '',
    public returnQuantity: number = 0,
    public returnAmount: number = 0,
    public returnReason: string = '',
    public returnStatus: string = '',
    public inventoryCity: string = '',
    public deliveryCity: string = '',
    public requestedDate: string = '',
    public reviewedDate: string | null = null
  ) {}
}
export class OrderItemSummaryModel {

  constructor(

    public orderItemsId: number = 0,
    public sku: string = '',
    public productName: string = '',
    public vendorName: string = '',
    public quantity: number = 0,
    public unitPrice: number = 0,
    public discount: number = 0,

    public inventoryId: number = 0,
    public inventoryCity: string = '',
    public inventoryAddress: string = '',

    public itemTotal: number = 0,
    public orderItemStatus: string = ''

  ) { }

}
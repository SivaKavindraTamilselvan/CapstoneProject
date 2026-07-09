import { ReturnSummaryModel } from "../../user/order/return.order.model";

export class OrderItemModel {

  constructor(
    public orderItemsId: number = 0,
    public sku: string = '',
    public canReturn: boolean | null = null,
    public productName: string = '',
    public vendorName: string = '',
    public quantity: number = 0,
    public unitPrice: number = 0,
    public discount: number = 0,
    public inventoryId: number = 0,
    public inventoryCity: string = '',
    public inventoryAddress: string = '',
    public productImageUrl : string ='',
    public itemTotal: number = 0,
    public orderItemStatus: string = '',
    public returns : ReturnSummaryModel[] = [] 
  ) { }

}
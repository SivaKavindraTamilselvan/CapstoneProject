export class OrderItemModel {

  constructor(
    public orderItemsId: number = 0,
    public sku: string = '',
    public productName: string = '',
    public vendorName: string = '',
    public quantity: number = 0,
    public unitPrice: number = 0,
    public totalPrice: number = 0
  ) { }

}
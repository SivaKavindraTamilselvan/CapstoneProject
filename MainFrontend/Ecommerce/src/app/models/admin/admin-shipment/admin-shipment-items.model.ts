export class ShipmentItemModel {
  constructor(
    public shipmentItemsId: number = 0,
    public orderItemsId: number = 0,

    public productName: string = '',
    public sku: string = '',

    public quantity: number = 0,
    public unitPrice: number = 0
  ) {}
}
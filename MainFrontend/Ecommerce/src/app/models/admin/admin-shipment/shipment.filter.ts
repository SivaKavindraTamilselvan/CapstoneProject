export class ShipmentFilter {
  constructor(
    public shipmentTypeId?: number,
    public shipmentStatusId?: number,
    public orderId?: number ,

    public courierName?: string,
    public pickUpAddressId?: number,

    public trackingNumber?: string,

    public fromDate?: string ,
    public toDate?: string ,

    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}
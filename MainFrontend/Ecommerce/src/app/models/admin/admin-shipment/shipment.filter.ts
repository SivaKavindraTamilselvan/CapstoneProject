export class ShipmentFilter {
  constructor(
    public shipmentTypeId: number | null = null,
    public shipmentStatusId: number | null = null,
    public orderId: number | null = null,

    public courierName: string = '',
    public pickUpAddressId: number | null = null,

    public trackingNumber: string = '',

    public onGoingShipment: boolean | null = null,

    public fromDate: string = '',
    public toDate: string = '',

    public pageNumber: number = 1,
    public pageSize: number = 10
  ) { }
}
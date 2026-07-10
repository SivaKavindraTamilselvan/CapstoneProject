export class ShipmentUpdateRequestModel {
  constructor(
    public shipmentId: number = 0,
    public shipmentStatusId: number = 0,
    public location: string = '',
    public remarks: string = ''
  ) {}
}
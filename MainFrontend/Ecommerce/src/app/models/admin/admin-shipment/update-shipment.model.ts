export class ShipmentUpdateModel {
  constructor(
    public shipmentId: number = 0,
    public shipmentStatusId: string = '',
    public location: string = '',
    public remarks: string = ''
  ) {}
}
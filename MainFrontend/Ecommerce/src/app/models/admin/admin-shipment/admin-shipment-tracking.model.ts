export class ShipmentTrackingModel {
  constructor(
    public shipmentTrackingId: number = 0,

    public status: string = '',
    public location: string = '',
    public remarks: string = '',

    public updatedAt: string = ''
  ) {}
}
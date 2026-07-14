import { ShipmentItemModel } from "./admin-shipment-items.model";
import { ShipmentTrackingModel } from "./admin-shipment-tracking.model";

export class ShipmentModel {
  constructor(
    public shipmentId: number = 0,
    public shipmentTypeId: number = 0,
    public orderId: number = 0,

    public currentStatus: string = '',
    public trackingNumber: string = '',

    public shippingCharge: number = 0,

    public expectedDeliveryDate: string | null = null,
    public shippedDate: string | null = null,
    public deliveryDate: string | null = null,

    public createdAt: string = '',

    public shipperId: number | null = null,
    public shipperName: string = '',

    public pickupAddress: string = '',

    public customerName: string = '',
    public customerEmail: string = '',

    public items: ShipmentItemModel[] = [],
    public tracking: ShipmentTrackingModel[] = []
  ) {}
}
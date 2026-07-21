import { CancelSummaryModel } from "../../user/order/cancel.order.model";
import { ReturnSummaryModel } from "../../user/order/return.order.model";

export class OrderItemModel {

  constructor(
    public orderId: number = 0,

    public orderItemsId: number = 0,
    public sku: string = '',
    public canReturn: boolean | null = null,
    public productName: string = '',
    public productVariantId: number = 0,
    public vendorName: string = '',
    public quantity: number = 0,
    public wallet: number = 0,
    public unitPrice: number = 0,
    public coupon: number = 0,
    public overallCost: number = 0,
    public shippingCharge: number = 0,
    public discount: number = 0,
    public inventoryId: number = 0,
    public inventoryCity: string = '',
    public inventoryAddress: string = '',
    public inventoryLandMark: string = '',
    public inventoryState: string = '',
    public inventoryPincode: string = '',
    public productImageUrl: string = '',
    public itemTotal: number = 0,
    public orderItemStatus: string = '',
    public contactPersonName: string = '',
    public contactPersonPhoneNumber: string = '',
    public vendorContactPersonName: string = '',
    public vendorContactPersonPhoneNumber: string = '',
    public userAddress: string = '',
    public userLandMark: string = '',
    public userCity: string = '',
    public userPincode: string = '',
    public returns: ReturnSummaryModel[] = [],
    public cancels: CancelSummaryModel[] = []
  ) { }

}
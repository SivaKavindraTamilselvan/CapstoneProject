import { OrderItemModel } from "./get-orderitem.model";

export class OrderModel {

  constructor(

    public orderId: number = 0,
    public userId: number = 0,
    public orderNumber: string = '',
    public userName: string = '',
    public userAddress: string = '',
    public userLandMark: string = '',
    public userCity: string = '',
    public userPincode: string = '',
    public userState: string = '',

    public orderDate: string = '',

    public orderStatus: string = '',

    public totalProductAmount: number = 0,
    public totalShippingAmount: number = 0,
    public totalCouponAmount: number = 0,
    public finalAmount: number = 0,

    public totalItems: number = 0,
    public paymentStatus : string = '',
    public paymentType :string = '',

    public orderItems: OrderItemModel[] = []

  ) { }

}

export class ReturnSummaryModel {
  returnId: number = 0;
  productImageUrl: string = '';
  productName: string = '';
  sku: string = '';
  vendorName: string = '';
  returnQuantity: number = 0;
  returnAmount: number = 0;
  returnReason: string = '';
  returnStatus: string = '';
  inventoryCity: string = '';
  deliveryCity: string = '';
  requestedDate: Date = new Date();
  reviewedDate: Date | null = null;
}

export class RequestAdminReturnFilter {
  onGoing : boolean | null = null;
  returnStatusId: number | null = null;
  returnReasonId: number | null = null;
  vendorId: number | null = null;
  orderItemId: number | null = null;
  orderId: number | null = null;
  fromDate: string | null = null;
  toDate: string | null = null;
  pageNumber: number = 1;
  pageSize: number = 10;
}

export class AdminCreateReturnRefund {
  returnId: number = 0;
  refundAmount: number = 0;
  remarks: string = '';
}
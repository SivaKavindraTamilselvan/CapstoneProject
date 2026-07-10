import { OrderItemModel } from "./get-orderitem.model";

export class OrderModel {

  constructor(

    public orderId: number = 0,
    public orderNumber: string = '',
    public userName: string = '',

    public orderDate: string = '',

    public orderStatus: string = '',

    public totalProductAmount: number = 0,
    public totalShippingAmount: number = 0,
    public totalCouponAmount: number = 0,
    public finalAmount: number = 0,

    public totalItems: number = 0,

    public orderItems: OrderItemModel[] = []

  ) { }

}
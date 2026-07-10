export class CouponListModel {
  constructor(
    public couponId: number,
    public couponCode: string,
    public discountValue: number,
    public minimumOrderAmount: number,
    public startDate: Date,
    public endDate: Date,
    public minimumNumberOfUsage: number,
    public isActive: boolean,
    public isExpired: boolean,
    public couponTypeName: string,
    public usageCount: number,
    public createdAt: Date
  ) {}
}

export class CouponUsageModel {
  constructor(
    public couponUsageId: number,
    public orderId: number,
    public orderNumber: string,
    public userId: number,
    public userName: string,
    public orderFinalAmount: number,
    public usedAt: Date
  ) {}
}

export class CouponDetailModel {
  constructor(
    public couponId: number = 0,
    public couponCode: string = '',
    public couponDescription: string = '',
    public discountValue: number = 0,
    public minimumOrderAmount: number = 0,
    public startDate: Date = new Date(),
    public endDate: Date = new Date(),
    public minimumNumberOfUsage: number = 0,
    public isActive: boolean = false,
    public isExpired: boolean = false,
    public couponTypeId: number = 0,
    public couponTypeName: string = '',
    public createdByUserId: number = 0,
    public createdByUserName: string = '',
    public createdAt: Date = new Date(),
    public updatedAt: Date | null = null,
    public usageCount: number = 0,
    public applicableProductIds: number[] = [],
    public usageHistory: CouponUsageModel[] = []
  ) {}
}
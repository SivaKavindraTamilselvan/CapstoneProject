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
    public couponId: number,
    public couponCode: string,
    public couponDescription: string,
    public discountValue: number,
    public minimumOrderAmount: number,
    public startDate: Date,
    public endDate: Date,
    public minimumNumberOfUsage: number,
    public isActive: boolean,
    public isExpired: boolean,
    public couponTypeId: number,
    public couponTypeName: string,
    public createdByUserId: number,
    public createdByUserName: string,
    public createdAt: Date,
    public updatedAt: Date | null,
    public usageCount: number,
    public applicableProductIds: number[],
    public usageHistory: CouponUsageModel[]
  ) {}
}
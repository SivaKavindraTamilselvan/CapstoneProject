export class UpdateCouponModel {
  constructor(
    public couponId : number = 0,
    public discountValue: number = 0,
    public minimumOrderAmount: number = 0,
    public startDate: string = '',
    public endDate: string = '',
    public minimumNumberOfUsage: number = 0,
    public couponDescription: string = ''
  ) {}
}
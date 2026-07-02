export class AdminCouponFilter {
  constructor(
    public pageNumber: number = 1,
    public pageSize: number = 10,
    public couponId: number | null = null,
    public search: string | null = null,
    public couponTypeId: number | null = null,
    public isActive: boolean | null = null,
    public isExpired: boolean | null = null,
    public validFrom: Date | null = null,
    public validTo: Date | null = null,
    public minDiscountValue: number | null = null,
    public maxDiscountValue: number | null = null,
    public minOrderAmount: number | null = null,
    public maxOrderAmount: number | null = null
  ) {}
}
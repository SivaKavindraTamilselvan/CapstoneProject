export class AddCouponModel {
    constructor(
        public couponCode: string = '',
        public discountValue: number = 0,
        public minimumOrderAmount: number = 0,
        public startDate: string = '',
        public endDate: string = '',
        public minimumNumberOfUsage: number = 1,
        public couponDescription: string = ''
    ) { }
}
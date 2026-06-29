export class UserCouponModel {
    constructor(
        public couponId : number = 0,
        public couponCode : string = '',
        public discountValue : number = 0,
        public minimumOrderAmount : number = 0,
        public startDate : Date,
        public endDate : Date,
        public minimumNumberOfUsage : number = 0
    ) { }

}
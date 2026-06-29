export class AddUserOrderModel {
    constructor(
        public addressId: number,
        public couponId: number | null = null,
        public paymentMethod: number,
    ) { }
}
export class AddUserOrderModel {
    constructor(
        public addressId: number,
        public couponId: number | null = null,
        public paymentMethod: number,
    ) { }
}

export class CreateReview {
    constructor(
        public orderDetailsId: number | null = null,
        public reviewDescriptionId: number | null = null,
        public additionalReviewDescription: string = '',
        public starId: number | null = null
    ) { }
}
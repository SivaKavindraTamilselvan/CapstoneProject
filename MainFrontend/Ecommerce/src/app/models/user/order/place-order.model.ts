export class AddUserOrderModel {
    constructor(
        public addressId: number,
        public couponId: number | null = null,
        public paymentMethod: number,
        public useWallet : boolean | null = null,
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

export interface ReviewItem {
  reviewId: number;
  starId: number;
  reviewDescription: string;
  additionalReviewDescription?: string;
  userName?: string;
  createdAt: string;
}

export interface ProductReviewSummary {
  productId: number;
  averageRating: number;
  totalReviews: number;
  starBreakdown: Record<number, number>;
  reviews: ReviewItem[];
}
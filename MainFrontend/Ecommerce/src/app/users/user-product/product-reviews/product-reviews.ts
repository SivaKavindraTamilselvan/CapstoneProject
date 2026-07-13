import { Component, effect, input, signal } from '@angular/core';
import { ProductReviewSummary } from '../../../models/user/order/place-order.model';
import { UserOrderService } from '../../../services/user-order.Service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-product-reviews',
  imports: [DatePipe],
  templateUrl: './product-reviews.html',
  styleUrl: './product-reviews.css',
})
export class ProductReviews {
  productId = input.required<number>();

  protected loading = signal(true);
  protected summary = signal<ProductReviewSummary | null>(null);
  protected Math = Math;

  constructor(private reviewService: UserOrderService) {
    effect(() => {
      const id = this.productId();
      this.loading.set(true);
      this.reviewService.getProductReviews(id).subscribe({
        next: (data) => {
          this.summary.set(data);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    });
  }
}
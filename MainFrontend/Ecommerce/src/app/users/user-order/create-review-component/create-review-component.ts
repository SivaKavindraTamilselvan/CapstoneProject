import { Component, computed, input, output, signal } from '@angular/core';
import { CreateReview } from '../../../models/user/order/place-order.model';
import { form, FormField, required } from '@angular/forms/signals';
import { UserOrderService } from '../../../services/user-order.Service';
import { FormsModule, NgModel, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-review-component',
  imports: [FormsModule, FormField, ReactiveFormsModule],
  templateUrl: './create-review-component.html',
  styleUrl: './create-review-component.css',
})
export class CreateReviewComponent {
  orderDetailsId = input.required<number>();
  close = output<void>();
  progress = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  reviewModel = signal(new CreateReview());
  selectedStars = computed(() => this.reviewForm.starId().value() ?? 0);
  isOpen = signal<boolean>(false);


  constructor(private reviewService: UserOrderService) { }
  reviewForm = form(this.reviewModel, (path) => {
    required(path.reviewDescriptionId, { message: 'Please Choose The Review Description' });
    required(path.starId, { message: 'Please select the star rating' });
  });
  submitReview() {
    this.errorMessage.set(null);
    if (this.reviewForm().invalid()) {
      this.errorMessage.set('Please complete all fields.');
      return;
    }
    this.reviewModel.update(model => ({
      ...model,
      orderDetailsId: this.orderDetailsId()
    }));
    this.progress.set(true);
    this.reviewService.createReview(this.reviewModel()).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set('Review submitted successfully.');
        this.close.emit();
      },
      error: () => {
        this.progress.set(false);
        this.errorMessage.set('Already reviewed for this order item.');
      }
    });
  }
  closePopup() {
    this.close.emit();
  }
  
  setStar(star: number): void {
    this.reviewForm.starId().value.set(star);
  }

  onDescriptionChange(event : Event) : void {
    const value = (event.target as HTMLSelectElement).value;
  this.reviewModel.update(model => ({
    ...model,
    reviewDescriptionId: value ? Number(value) : null
  }));
}
}

import { CommonModule } from '@angular/common';
import { Component, input, output, signal } from '@angular/core';
import { VendorOrderService } from '../../../services/vendor-order.Service'; 

@Component({
  selector: 'app-review-return-product',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review-return-product.html',
  styleUrl: './review-return-product.css',
})
export class ReviewReturnProductPopup {
  returnId = input.required<number>();

  closed = output<void>();
  accepted = output<void>();
  requestAdditional = output<number>(); // emits returnId to parent

  progress = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  constructor(private vendorReturnService: VendorOrderService) {}

  close() {
    this.closed.emit();
  }

  acceptAndRefund() {
    this.progress.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.vendorReturnService.acceptReturnProduct(this.returnId()).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set('Return accepted and refund created successfully');

        setTimeout(() => {
          this.accepted.emit();
          this.closed.emit();
        }, 1500);
      },
      error: (error) => {
        this.progress.set(false);
        this.errorMessage.set(
          error.error?.message ?? 'Something went wrong. Please try again.'
        );
      },
    });
  }

  openAdditionalRefund() {
    this.requestAdditional.emit(this.returnId());
    this.closed.emit();
  }
}
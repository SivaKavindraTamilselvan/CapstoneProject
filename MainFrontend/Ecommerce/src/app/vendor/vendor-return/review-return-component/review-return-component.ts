import { Component, input, output, signal } from '@angular/core';
import { VendorOrderService } from '../../../services/vendor-order.Service';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-review-return-component',
  imports: [NgClass],
  templateUrl: './review-return-component.html',
  styleUrl: './review-return-component.css',
})
export class ReviewReturnComponent {
  returnId = input.required<number>();
  decision = input<boolean>(true); // true = approve, false = reject

  closed = output<void>();
  reviewed = output<void>();

  progress = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  constructor(private vendorReturnService: VendorOrderService) {}

  close() {
    this.closed.emit();
  }

  submit() {
    this.progress.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const request = {
      returnId: this.returnId(),
      review: this.decision(),
    };

    this.vendorReturnService.reviewReturn(request).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set(
          this.decision() ? 'Return approved successfully' : 'Return rejected successfully'
        );

        setTimeout(() => {
          this.reviewed.emit();
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
}

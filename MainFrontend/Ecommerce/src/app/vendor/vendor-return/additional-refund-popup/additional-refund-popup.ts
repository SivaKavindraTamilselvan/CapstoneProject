import { CommonModule } from '@angular/common';
import { Component, input, output, signal } from '@angular/core';
import { FormField, form, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { VendorOrderService } from '../../../services/vendor-order.Service'; 

class AdditionalRefundModel {
  constructor(
    public returnId: number = 0,
    public damageCost: number = 0,
    public remarks: string = ''
  ) {}
}

@Component({
  selector: 'app-additional-refund-popup',
  standalone: true,
  imports: [CommonModule, FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './additional-refund-popup.html',
  styleUrl: './additional-refund-popup.css',
})
export class AdditionalRefundPopup {
  returnId = input.required<number>();

  closed = output<void>();
  submitted = output<void>();

  progress = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  refundModel = signal(new AdditionalRefundModel());

  refundForm = form(this.refundModel, (path) => {
    required(path.remarks, {
      message: 'Enter remarks for this refund request',
    });
  });

  constructor(private vendorReturnService: VendorOrderService) {}

  ngOnInit() {
    this.refundModel.set(new AdditionalRefundModel(this.returnId(), 0, ''));
  }

  onDamageCostInput(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.refundForm.damageCost().value.set(value ? Number(value) : 0);
  }

  close() {
    this.closed.emit();
  }

  submit() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.refundForm().invalid()) {
      this.errorMessage.set('Enter proper details');
      return;
    }

    this.progress.set(true);

    const request = {
      returnId: this.refundModel().returnId,
      damageCost: Number(this.refundModel().damageCost),
      remarks: this.refundModel().remarks,
    };

    this.vendorReturnService.reviewReturnProduct(request).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set('Additional refund requested successfully');

        setTimeout(() => {
          this.submitted.emit();
          this.closed.emit();
        }, 1500);
      },
      error: (error) => {
        this.progress.set(false);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(', ');
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(
            error.error?.message ?? 'Something went wrong. Please try again.'
          );
        }
      },
    });
  }
}
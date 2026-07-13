import { Component, signal } from '@angular/core';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { AdminCreateReturnRefund } from '../../../models/admin/admin-orders/get-order.model';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-return-refund-popup',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './return-refund-popup.html',
  styleUrl: './return-refund-popup.css',
})
export class ReturnRefundPopup {

  constructor(private adminOrderService: AdminOrderService) {}

  show = signal(false);
  progress = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  refundModel = signal(new AdminCreateReturnRefund());

  refundForm = form(this.refundModel, (path) => {
    required(path.refundAmount, { message: 'Enter The Refund Amount' });
    min(path.refundAmount, 1, { message: 'Refund Amount Must Be Greater Than 0' });
    required(path.remarks, { message: 'Enter The Remarks' });
    maxLength(path.remarks, 150, { message: 'Maximum 150 characters' });
  });

  openPopup(returnId: number, suggestedAmount: number | null = null) {
    this.errorMessage.set('');
    this.successMessage.set(null);
    this.refundModel.update(model => ({
      ...model,
      returnId: returnId,
      refundAmount: suggestedAmount ?? 0,
      remarks: ''
    }));
    this.show.set(true);
  }

  onConfirm() {
    this.errorMessage.set('');
    this.successMessage.set(null);

    const errors = [];
    if (this.refundForm.refundAmount().invalid()) {
      errors.push(this.refundForm.refundAmount().errors()[0].message);
    }
    if (this.refundForm.remarks().invalid()) {
      errors.push(this.refundForm.remarks().errors()[0].message);
    }
    this.errorMessage.set(errors.join(", "));

    if (this.refundForm().invalid()) {
      return;
    }

    this.progress.set(true);

    const request: AdminCreateReturnRefund = {
      returnId: this.refundModel().returnId,
      refundAmount: Number(this.refundModel().refundAmount),
      remarks: this.refundModel().remarks
    };

    this.adminOrderService.createReturnOrdersDetails(request).subscribe({
      next: () => {
        this.successMessage.set("Refund processed successfully");
        setTimeout(() => {
          this.onCancel();
          this.successMessage.set(null);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error.error?.message ?? "Something went wrong. Please try again.");
        }
        this.progress.set(false);
      }
    });
  }

  onCancel() {
    this.show.set(false);
    this.refundForm().reset();
    this.refundModel.set(new AdminCreateReturnRefund());
    this.errorMessage.set('');
    this.successMessage.set(null);
    this.progress.set(false);
  }
}

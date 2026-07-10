import { Component, input, output, signal } from '@angular/core';
import { AddReturnModel } from '../../../models/user/order/return.order.model';
import { form, FormField, required } from '@angular/forms/signals';
import { UserOrderService } from '../../../services/user-order.Service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-return-component',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './add-return-component.html',
  styleUrl: './add-return-component.css',
})
export class AddReturnComponent {
  orderItemId = input.required<number>();

  closed = output<void>();
  returned = output<void>();

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  returnReasons = [
    { id: 1, name: 'Damaged Product' },
    { id: 2, name: 'Wrong Product Delivered' },
    { id: 3, name: 'Product Not As Described' },
    { id: 4, name: 'Defective Product' },
    { id: 5, name: 'Size Does Not Fit' },
    { id: 6, name: 'Color Mismatch' },
    { id: 7, name: 'Received Incomplete Item' },
    { id: 8, name: 'Ordered By Mistake' },
    { id: 9, name: 'Delivery Took Too Long' },
    { id: 10, name: 'Found Better Alternative' },
    { id: 11, name: 'Quality Not Satisfactory' },
    { id: 12, name: 'Changed Mind' },
  ];

  returnModel = signal(new AddReturnModel());

  returnForm = form(this.returnModel, (path) => {
    required(path.returnReasonId, {
      message: 'Select the return reason',
    });

    required(path.additionalReason, {
      message: 'Enter the additional reason',
    });

    required(path.returnQuantity, {
      message: 'Enter the return quantity',
    });
  });

  constructor(private userOrderService: UserOrderService) {}

  ngOnInit() {
    this.returnModel.set(
      new AddReturnModel(
        0,                  // returnReasonId
        this.orderItemId(), // orderItemId
        '',                 // additionalReason
        1                   // returnQuantity
      )
    );
  }

  onReasonChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.returnForm.returnReasonId().value.set(value ? Number(value) : 0);
  }

  onQuantityInput(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.returnForm.returnQuantity().value.set(value ? Number(value) : 1);
  }

  onCancel() {
    this.closed.emit();
  }

  onConfirm() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.returnForm().invalid()) {
      this.errorMessage.set('Enter proper details');
      return;
    }

    const request = {
      returnReasonId: Number(this.returnModel().returnReasonId),
      orderItemId: this.returnModel().orderItemId,
      additionalReason: this.returnModel().additionalReason,
      returnQuantity: Number(this.returnModel().returnQuantity),
    };

    this.userOrderService.createReturn(request).subscribe({
      next: () => {
        this.successMessage.set('Return request submitted successfully');

        setTimeout(() => {
          this.returned.emit();
          this.closed.emit();
        }, 2000);
      },
      error: (error) => {
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(', ');

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

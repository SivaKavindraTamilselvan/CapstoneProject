import { Component, effect, input, output, signal } from '@angular/core';
import { AdminCouponService } from '../../../services/admin-coupon.Service';
import { UpdateCouponModel } from '../../../models/admin/admin-coupon/update-coupon.model';
import { form, FormField, min } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-coupon-component',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './update-coupon-component.html',
  styleUrl: './update-coupon-component.css',
})
export class UpdateCouponComponent {
  constructor(private adminCouponService: AdminCouponService) {
    effect(() => {
      if (this.updateCouponForm().invalid()) {
        this.formErrorMessage.set('Please fix the validation errors.');
      } else {
        this.formErrorMessage.set(null);
      }
    });

    effect(() => {
      const id = this.couponId();
      if (id != null) {
        this.loadCouponDetails(id);
      }
    });
  }

  couponId = input<number | null>(null);

  closed = output<void>();
  updated = output<void>();

  updateCouponModel = signal(new UpdateCouponModel());
  progress = signal(false);
  formErrorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  updateErrorMessage = signal<string | null>(null);

  updateCouponForm = form(this.updateCouponModel, (path) => {
    min(path.discountValue, 0, { message: 'Discount value cannot be negative.' });
    min(path.minimumOrderAmount, 0, { message: 'Minimum order amount cannot be negative.' });
    min(path.minimumNumberOfUsage, 0, { message: 'Minimum number of usage cannot be negative.' });
  });

  private loadCouponDetails(couponId: number): void {
    this.progress.set(true);
    this.adminCouponService.getCouponById(couponId).subscribe({
      next: (response: any) => {
        this.updateCouponModel.set(
          new UpdateCouponModel(
            response.couponId,
            response.discountValue,
            response.minimumOrderAmount,
            response.startDate?.split('T')[0],
            response.endDate?.split('T')[0],
            response.minimumNumberOfUsage,
            response.couponDescription
          )
        );
        this.progress.set(false);
      },
      error: (error) => {
        console.log(error);
        this.progress.set(false);
        this.updateErrorMessage.set(error.error?.message ?? 'Failed to load coupon details.');
      }
    });
  }

  updateCoupon(): void {
    if (this.updateCouponForm().invalid()) {
      this.formErrorMessage.set('Please fix the validation errors.');
      return;
    }

    this.progress.set(true);
    this.adminCouponService.updateCoupon(this.updateCouponModel()).subscribe({
      next: (response) => {
        this.progress.set(false);
        this.successMessage.set('Coupon updated successfully.');
        this.updated.emit();
        this.close();
      },
      error: (err) => {
        console.error('Failed to update coupon', err);
        this.progress.set(false);
        if (err.status === 400 && err.error?.errors) {
          const messages = Object.values(err.error.errors).flat().join(', ');
          this.updateErrorMessage.set(messages);
        } else {
          this.updateErrorMessage.set(err.error?.message ?? 'Something went wrong.');
        }
      }
    });
  }

  close(): void {
    this.updateCouponModel.set(new UpdateCouponModel());
    this.formErrorMessage.set(null);
    this.updateErrorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}

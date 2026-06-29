import { Component, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormField, form, required, minLength } from '@angular/forms/signals';
import { Router } from '@angular/router';

import { AddCouponModel } from '../../../models/admin/admin-coupon/add-coupon.model';
import { AdminCouponService } from '../../../services/admin-coupon.Service';

@Component({
  selector: 'app-add-coupon',
  imports: [FormsModule, ReactiveFormsModule, FormField],
  templateUrl: './add-coupon.html',
  styleUrl: './add-coupon.css'
})
export class AddCoupon {

  errorMessage = signal<string | null>(null);
  progress = signal(false);

  couponModel = signal(new AddCouponModel());

  constructor(
    private router: Router,
    private couponService: AdminCouponService
  ) { }

  couponForm = form(this.couponModel, (path) => {

    required(path.couponCode, {
      message: 'Coupon Code is required'
    });

    minLength(path.couponCode, 3, {
      message: 'Coupon Code must contain at least 3 characters'
    });

    required(path.discountValue, {
      message: 'Discount Value is required'
    });

    required(path.minimumOrderAmount, {
      message: 'Minimum Order Amount is required'
    });

    required(path.startDate, {
      message: 'Start Date is required'
    });

    required(path.endDate, {
      message: 'End Date is required'
    });

    required(path.minimumNumberOfUsage, {
      message: 'Minimum Usage is required'
    });

    required(path.couponDescription, {
      message: 'Coupon Description is required'
    });

  });

  handleAddCouponClick() {

    if (this.couponForm().invalid()) {
      alert("Enter Proper Details");
      return;
    }

    this.progress.set(true);

    this.couponService.addCoupon(this.couponModel()).subscribe({

      next: () => {

        alert("Coupon Added Successfully");

        this.progress.set(false);

        this.couponModel.set(new AddCouponModel());

        this.errorMessage.set(null);

      },

      error: (error) => {

        console.error(error);

        this.progress.set(false);

        if (error.status === 409) {

          this.errorMessage.set(error.error.message);

        }
        else if (error.status === 400 && error.error?.errors) {

          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);

        }
        else {

          this.errorMessage.set("Something went wrong. Please try again.");

        }

      }

    });

  }

  resetForm() {

    this.couponModel.set(new AddCouponModel());

    this.errorMessage.set(null);

  }

}
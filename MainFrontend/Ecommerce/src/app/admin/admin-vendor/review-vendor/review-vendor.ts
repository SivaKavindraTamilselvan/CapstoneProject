import { Component, signal } from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin-vendor.model';
import { ReviewVendorModel } from '../../../models/review.vendor.model';
import { FormField, form, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-review-vendor',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './review-vendor.html',
  styleUrl: './review-vendor.css',
})
export class ReviewVendor {
  vendors = signal<PagedResponse<AdminVendorModel> | null>(null);
  showActivatePopup = signal(false);
  selectedVendorId = signal<number | null>(null);
  reviewVendorModel = signal(new ReviewVendorModel());
  constructor(private route: Router, private adminVendorService: AdminVendorService) {

  }

  ngOnInit(): void {
    this.loadPendingVendor();
  }

  reviewForm = form(this.reviewVendorModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    pattern(path.approvalStatusId, /^[23]$/, { message: "Select valid approval status" })
  })

  loadPendingVendor() {
    this.adminVendorService.getPendingVendor().subscribe({
      next: (response: any) => {
        this.vendors.set(response);

      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.vendors.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
        else {
          console.log(error);
        }
      }
    })
  }
  openReviewPopup(vendorId: number) {
    this.selectedVendorId.set(vendorId);

    this.reviewVendorModel.set(
      new ReviewVendorModel(vendorId, "", "")
    );

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedVendorId.set(null);
    this.reviewVendorModel.set(new ReviewVendorModel());
  }
  handleReview() {
    if (this.reviewForm().invalid()) {
      alert("Enter proper details");
      return;
    }

    const request = {
      vendorId: this.reviewVendorModel().vendorId,
      approvalStatusId: Number(this.reviewVendorModel().approvalStatusId),
      remark: this.reviewVendorModel().remark
    };

    this.adminVendorService.reviewVendor(request).subscribe({
      next: () => {
        alert("Vendor reviewed successfully");
        this.closePopup();
        this.loadPendingVendor();
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
}

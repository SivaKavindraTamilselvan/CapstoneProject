import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminProductService } from '../../../services/admin-product.Service';
import { form, maxLength, required } from '@angular/forms/signals';
import { UpdateProductComponenet } from '../../../shared-components/update-product-componenet/update-product-componenet';
import { DeletePopupComponents } from '../../../shared-components/delete-popup-components/delete-popup-components';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';
import { AdminDeleteProductModel, AdminDeleteVariantModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { ReviewProductVariantModel } from '../../../models/product/review-variant.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-variant-details',
  imports: [ReviewPopupComponent, DeletePopupComponents, DatePipe, DecimalPipe, NgClass],
  templateUrl: './admin-variant-details.html',
  styleUrl: './admin-variant-details.css',
})
export class AdminVariantDetails {
  variantModel = signal<VendorProductVariantModel | null>(null);
  currentImageIndex = signal(0);

  constructor(private route: Router, private adminProductService: AdminProductService, private router: ActivatedRoute) {

  }

  ngOnInit(): void {
    const productId = Number(this.router.snapshot.paramMap.get('id'));

    if (productId) {
      this.loadProductDetails(productId);
    }
  }

  loadProductDetails(productId: number) {
    this.adminProductService.getProductVariantDetails(productId).subscribe({
      next: (response: any) => {
        console.log(response);
        this.variantModel.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  prevImage() {
    const images = this.variantModel()?.productImages;
    if (!images?.length) return;

    this.currentImageIndex.update(i => (i === 0 ? images.length - 1 : i - 1));
  }

  nextImage() {
    const images = this.variantModel()?.productImages;
    if (!images?.length) return;

    this.currentImageIndex.update(i => (i === images.length - 1 ? 0 : i + 1));
  }

  errorMessage = signal('');
  successMessage = signal('');
  showReviewPopup = signal(false);
  reviewProductModel = signal(new ReviewProductVariantModel());
  progress = signal(false);
  approvalStatusOption = [
    { id: 4, label: 'Accepted' },
    { id: 5, label: 'Rejected' },
  ];

  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remarks, { message: "Enter The Remarks" });
    maxLength(path.remarks, 150, { message: "Maximum 100 characters" });
  });

  selectedProductIdForUpdate = signal<number | null>(null);

  openReviewPopup(productVariantId: number) {
    this.selectedProductIdForUpdate.set(productVariantId);
    this.reviewProductModel.set(new ReviewProductVariantModel(productVariantId, null, ""));
    this.showReviewPopup.set(true);
  }

  onConfirmReview() {
    this.errorMessage.set('');
    this.successMessage.set('');
    const errors = [];
    if (this.reviewForm.approvalStatusId().invalid()) {
      errors.push(this.reviewForm.approvalStatusId().errors()[0].message);
    }
    if (this.reviewForm.remarks().invalid()) {
      errors.push(this.reviewForm.remarks().errors()[0].message);
    }
    this.errorMessage.set(errors.join(", "));
    if (this.reviewForm().invalid()) {
      return;
    }

    const id = this.variantModel()?.productVariantId;
    if (id == null) {
      return;
    }
    const request = {
      productVariantId: this.reviewProductModel().productVariantId,
      approvalStatusId: Number(this.reviewProductModel().approvalStatusId),
      remarks: this.reviewProductModel().remarks
    };
    this.adminProductService.reviewProductVariant(request).subscribe({
      next: () => {
        this.successMessage.set("Product Variant reviewed successfully");
        setTimeout(() => {
          this.onCancelReview();
          this.successMessage.set('');
          this.loadProductDetails(id);
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set('');
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.error?.message ?? "Something went wrong. Please try again.");
        }
        this.progress.set(false);
      }
    });
  }
  onCancelReview() {
    this.showReviewPopup.set(false);
    this.reviewForm().reset();
    this.reviewProductModel.set(new ReviewProductVariantModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  showActivatePopup = signal(false);
  showUpdatePopup = signal(false);
  showUpdateRejectedPopup = signal(false);

  showDeletePopup = signal(false);

  deleteProductModel = signal(new AdminDeleteVariantModel());

  deleteForm = form(this.deleteProductModel, (path) => {
    required(path.remark, { message: "Enter The Remarks" });
  })

  openDeletePopup(productId: number) {
    this.deleteProductModel.update(model => ({
      ...model,
      productVariantId: productId,
      remark: ''
    }));
    this.showDeletePopup.set(true);
  }

  onConfirmDelete() {
    const id = this.variantModel()?.productVariantId;
    if (id == null) {
      return;
    }
    this.errorMessage.set('');
    this.successMessage.set('');

    const errors = [];
    if (this.deleteForm.remark().invalid()) {
      errors.push(this.deleteForm.remark().errors()[0].message);
    }
    this.errorMessage.set(errors.join(', '));

    if (this.deleteForm().invalid()) {
      return;
    }
    this.progress.set(true);

    const request = {
      productVariantId: this.deleteProductModel().productVariantId,
      remark: this.deleteProductModel().remark
    };

    this.adminProductService.deleteProductVariant(request).subscribe({
      next: () => {
        this.successMessage.set("Product deleted successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.onCancelDelete();
          this.successMessage.set('');
          this.loadProductDetails(id);
          this.progress.set(false);
        }, 3000);

      },
      error: (error) => {
        this.successMessage.set('');
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.error?.message ?? "Something went wrong. Please try again.");
        }
        this.progress.set(false);
      }
    });
  }

  onCancelDelete() {
    this.showDeletePopup.set(false);
    this.deleteForm().reset();
    this.deleteProductModel.set(new AdminDeleteVariantModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }
  goBack(): void {
    this.route.navigate(['/admin/product-variant/list']);
  }
}


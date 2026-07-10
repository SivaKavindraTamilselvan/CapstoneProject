import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { ReviewProductVariantModel } from '../../../models/product/review-variant.model';
import { form, maxLength, required } from '@angular/forms/signals';
import { UpdateRejectVairantComponent } from '../update-reject-vairant-component/update-reject-vairant-component';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { UpdateProductComponent } from '../../vendor-product/update-product-component/update-product-component';
import { DeleteProductComponent } from '../../vendor-product/delete-product-componentd/delete-product-componentd';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';

@Component({
  selector: 'app-vendor-variant-details',
  imports: [DatePipe, DecimalPipe, NgClass,UpdateRejectVairantComponent,UpdateProductComponent,DeleteProductComponent,ReviewPopupComponent],
  templateUrl: './vendor-variant-details.html',
  styleUrl: './vendor-variant-details.css',
})
export class VendorVariantDetails {
  variantModel = signal<VendorProductVariantModel | null>(null);
  currentImageIndex = signal(0);

  constructor(private route: Router, private vendorProductService: VendorProductService, private router: ActivatedRoute) {

  }

  ngOnInit(): void {
    const productId = Number(this.router.snapshot.paramMap.get('id'));

    if (productId) {
      this.loadProductDetails(productId);
    }
  }

  loadProductDetails(productId: number) {
    this.vendorProductService.getProductVariantDetails(productId).subscribe({
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
    { id: 2, label: 'Accepted' },
    { id: 3, label: 'Rejected' },
  ];

  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    maxLength(path.remark, 150, { message: "Maximum 100 characters" });
  });

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
    if (this.reviewForm.remark().invalid()) {
      errors.push(this.reviewForm.remark().errors()[0].message);
    }
    this.errorMessage.set(errors.join(", "));
    if (this.reviewForm().invalid()) {
      return;
    }

    const id = this.variantModel()?.productVariantId;
    if(id==null){
      return;
    }
    const request = {
      productVariantId: this.reviewProductModel().productVariantId,
      approvalStatusId: Number(this.reviewProductModel().approvalStatusId),
      remark: this.reviewProductModel().remark
    };
    this.vendorProductService.reviewProductVariant(request).subscribe({
      next: () => {
        this.successMessage.set("Vendor reviewed successfully");
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

  selectedProductIdForDelete = signal<number | null>(null);
  selectedProductIdForUpdate = signal<number | null>(null);
  selectedProductForUpdateRejected = signal<VendorProductVariantModel | null>(null);

  openDeletePopup(productVariantId: number) {
    this.selectedProductIdForDelete.set(productVariantId);
    this.showActivatePopup.set(true);
  }
  closeDeletePopup() {
    this.showActivatePopup.set(false);
    this.selectedProductIdForDelete.set(null);
  }

  openUpdatePopup(productVariantId: number) {
    this.selectedProductIdForUpdate.set(productVariantId);
    this.showUpdatePopup.set(true);
  }
  closeUpdatePopup() {
    this.showUpdatePopup.set(false);
    this.selectedProductIdForUpdate.set(null);
  }

  selectedVariantForUpdateRejected = signal<VendorProductVariantModel | null>(null);

  openUpdateRejectedPopup(variant: VendorProductVariantModel) {
    this.selectedVariantForUpdateRejected.set(variant);
    this.showUpdateRejectedPopup.set(true);
  }
  closeUpdateRejectedPopup() {
    this.showUpdateRejectedPopup.set(false);
    this.selectedVariantForUpdateRejected.set(null);
  }

}

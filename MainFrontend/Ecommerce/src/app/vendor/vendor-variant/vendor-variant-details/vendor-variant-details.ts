import { Component, computed, signal } from '@angular/core';
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
import { ProductImageModel } from '../../../models/product/product-image.model';
import { AddProductVariantImageModel } from '../../../models/vendor/vendor-product/add-model/add-variant-image.model';
import { AuthStateService } from '../../../services/auth-State.Service';

@Component({
  selector: 'app-vendor-variant-details',
  imports: [DatePipe, DecimalPipe, NgClass, UpdateRejectVairantComponent, UpdateProductComponent, DeleteProductComponent, ReviewPopupComponent],
  templateUrl: './vendor-variant-details.html',
  styleUrl: './vendor-variant-details.css',
})
export class VendorVariantDetails {
  variantModel = signal<VendorProductVariantModel | null>(null);
  currentImageIndex = signal(0);
  role = signal<string | undefined>(undefined);

  constructor(private route: Router, private vendorProductService: VendorProductService, private router: ActivatedRoute, private authService: AuthStateService) {

  }

  ngOnInit(): void {
    const productId = Number(this.router.snapshot.paramMap.get('id'));
    this.role.set(this.authService.getVendorRole());

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
        if (error.status === 401) {
          this.route.navigate(['/unauthorized']);
        }
        this.errorMessage.set(error.error.message);
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
    { id: 2, label: 'Accept' },
    { id: 3, label: 'Reject' },
  ];

  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remarks, { message: "Enter The Remarks" });
    maxLength(path.remarks, 150, { message: "Maximum 100 characters" });
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

  // ================== IMAGE MANAGEMENT ==================
  isImagePopupOpen = signal(false);
  isUploading = signal(false);
  isDeleting = signal<number | null>(null);
  isSettingMain = signal<number | null>(null);
  imageError = signal<string | null>(null);
  imageSuccess = signal<string | null>(null);

  // Fixed image angle types — same convention as add-variant
  imageAngles: { label: string; value: number }[] = [
    { label: 'Front', value: 1 },
    { label: 'Back', value: 2 },
    { label: 'Left', value: 3 },
    { label: 'Right', value: 4 },
  ];

  private nextAvailableAngle(): number {
    const usedAngles = (this.variantModel()?.productImages ?? []).map(i => i.displayOrder);
    const free = this.imageAngles.find((a) => !usedAngles.includes(a.value));
    return free ? free.value : 1;
  }

  imgAngleLabel(value: number): string {
    return this.imageAngles.find(a => a.value === value)?.label ?? '';
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const id = this.variantModel()?.productVariantId;
    if (id == null) return;

    this.imageError.set(null);
    this.imageSuccess.set(null);

    const file = input.files[0];
    const displayOrderId = this.nextAvailableAngle();

    this.isUploading.set(true);

    const formData = new FormData();
    formData.append('ProductVariantId', id.toString());
    formData.append('File', file);
    formData.append('DisplayOrderId', displayOrderId.toString());

    this.vendorProductService.uploadProductVariantImage(formData).subscribe({
      next: (res: any) => {
        const newImage: ProductImageModel = res.data ?? res;
        this.imageSuccess.set('Product Variant Image Added Successfully');

        this.variantModel.update(p => p ? {
          ...p,
          productImages: [...p.productImages, newImage]
        } : p);

        this.isUploading.set(false);
        input.value = '';
      },
      error: (error) => {
        if (error.error?.message) {
          this.imageError.set(error.error.message);
        } else if (error.status === 0) {
          this.imageError.set('Unable to upload image. Check your internet connection.');
        } else {
          this.imageError.set('Failed to upload image.');
        }
        this.isUploading.set(false);
      }
    });
  }

  deleteImage(productImageId: number): void {
    const target = this.variantModel()!.productImages.find(i => i.productImageId === productImageId);

    if (target?.isMainImage && this.variantModel()!.productImages.length > 1) {
      this.imageError.set('Set another image as main before deleting this one.');
      return;
    }

    this.isDeleting.set(productImageId);
    this.imageError.set(null);
    this.imageSuccess.set(null);

    this.vendorProductService.deleteProductImage(productImageId).subscribe({
      next: () => {
        this.imageSuccess.set('Product Variant Image Deleted Successfully');
        this.variantModel.update(p => p ? {
          ...p,
          productImages: p.productImages.filter(i => i.productImageId !== productImageId)
        } : p);
        this.isDeleting.set(null);
      },
      error: (error) => {
        if (error.error?.message) {
          this.imageError.set(error.error.message);
        } else if (error.status === 0) {
          this.imageError.set('Unable to delete image. Check your internet connection.');
        } else {
          this.imageError.set('Failed to delete image.');
        }
        this.isDeleting.set(null);
      }
    });
  }

  sortedProductImages = computed(() => {
    const imgs = this.variantModel()?.productImages ?? [];
    return [...imgs].sort((a, b) => {
      if (a.isMainImage !== b.isMainImage) return a.isMainImage ? -1 : 1;
      return a.displayOrder - b.displayOrder;
    });
  });


  goBack(): void {
    this.route.navigate(['/vendor/products/variants/list']);
  }

}

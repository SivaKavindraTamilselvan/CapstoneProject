import { Component, computed, signal } from '@angular/core';
import { ProductModel } from '../../../models/product/product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { CommonModule } from '@angular/common';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';
import { ProductImageModel } from '../../../models/product/product-image.model';
import { AddProductImageModel } from '../../../models/vendor/vendor-product/add-model/add-product-image';
import { MakeImageDefaultModel } from '../../../models/vendor/vendor-product/add-model/make-image-default';
import { AuthStateService } from '../../../services/auth-State.Service';
import { ReviewProductModel } from '../../../models/product/review-product.model';
import { form, maxLength, required } from '@angular/forms/signals';
import { AdminDeleteProductModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { PopupBase } from '../../../shared-class/popup-base-class';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { UpdateRejectedProductComponent } from '../update-rejected-product-component/update-rejected-product-component';
import { DeleteProductComponent } from '../delete-product-componentd/delete-product-componentd';
import { UpdateProductComponent } from '../update-product-component/update-product-component';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';
import { ProductReviews } from '../../../users/user-product/product-reviews/product-reviews';
import { ProductHistoryComponent } from '../../../admin/admin-product/product-history-component/product-history-component';

@Component({
  selector: 'app-vendor-product-details',
  imports: [CommonModule, UpdateRejectedProductComponent, UpdateProductComponent, DeleteProductComponent, ReviewPopupComponent, ProductReviews, ProductHistoryComponent],
  templateUrl: './vendor-product-details.html',
  styleUrl: './vendor-product-details.css',
})
export class VendorProductDetails extends PopupBase {
  productModel = signal<VendorProductModel | null>(null);
  currentImageIndex = signal(0);
  role = signal<string | undefined>(undefined);


  constructor(private vendorProductService: VendorProductService, private route: ActivatedRoute, private router: Router, private authService: AuthStateService) {
    super();
  }
  ngOnInit(): void {
    const productId = Number(this.route.snapshot.paramMap.get('id'));
    this.role.set(this.authService.getVendorRole());

    if (productId) {
      this.loadProductDetails(productId);
    }
  }

  tableLoading = signal(false);
  loadProductDetails(productId: number) {
    this.tableLoading.set(true);
    this.vendorProductService.getProductDetails(productId).subscribe({
      next: (response: any) => {
        //console.log(response);
        this.productModel.set(response);
        this.tableLoading.set(false);
      },
      error: (error) => {
        if (error.status === 401) {
          this.router.navigate(['/unauthorized']);
        }
        this.errorMessage.set(error.error.message);
        this.tableLoading.set(false);
      }
    })
  }

  protected loadData(): void {
    const id = this.productModel()?.productId;
    if (id == null) {
      return;
    }
    this.loadProductDetails(id);
  }


  prevImage() {
    const total = this.productModel()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i - 1 + total) % total);
  }

  nextImage() {
    const total = this.productModel()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i + 1) % total);
  }
  addVariant() {
    const product = this.productModel();
    if (!product) return;

    this.router.navigate(['/vendor/products', product.productId, 'variants', 'add'], {
      queryParams: {
        subCategoryId: product.productSubCategoryId,
        mainProductAttributeId: product.mainProductSubCategoryAttributeId,
      },
    });
  }

  isImagePopupOpen = signal(false);
  isUploading = signal(false);
  isDeleting = signal<number | null>(null);
  isSettingMain = signal<number | null>(null);
  imageError = signal<string | null>(null);
  imageSuccess = signal<string | null>(null);

  imageAngles: { label: string; value: number }[] = [
    { label: 'Front', value: 1 },
    { label: 'Back', value: 2 },
    { label: 'Left', value: 3 },
    { label: 'Right', value: 4 },
  ];

  private nextAvailableAngle(): number {
    const usedAngles = (this.productModel()?.productImages ?? []).map(i => i.displayOrder);
    const free = this.imageAngles.find((a) => !usedAngles.includes(a.value));
    return free ? free.value : 1;
  }

  imgAngleLabel(value: number): string {
    return this.imageAngles.find(a => a.value === value)?.label ?? '';
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const id = this.productModel()?.productId;
    if (id == null) return;

    this.imageError.set(null);
    this.imageSuccess.set(null);

    const file = input.files[0];
    const isFirstImage = (this.productModel()?.productImages.length ?? 0) === 0;
    const displayOrderId = this.nextAvailableAngle();

    this.isUploading.set(true);

    const formData = new FormData();
    formData.append('ProductId', id.toString());
    formData.append('File', file);
    formData.append('DisplayOrderId', displayOrderId.toString());
    formData.append('IsMainImage', isFirstImage.toString());

    this.vendorProductService.uploadProductImage(formData).subscribe({
      next: (res: any) => {
        const newImage: ProductImageModel = res.data ?? res;
        this.imageSuccess.set('Product Image Added Successfully');

        this.productModel.update(p => p ? {
          ...p,
          productImages: [...p.productImages, newImage]
        } : p);

        this.loadData();

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
    const target = this.productModel()!.productImages.find(i => i.productImageId === productImageId);

    if (target?.isMainImage && this.productModel()!.productImages.length > 1) {
      this.imageError.set('Set another image as main before deleting this one.');
      return;
    }

    this.isDeleting.set(productImageId);
    this.imageError.set(null);
    this.imageSuccess.set(null);

    this.vendorProductService.deleteProductImage(productImageId).subscribe({
      next: () => {
        this.imageSuccess.set('Product Image Deleted Successfully');
        this.productModel.update(p => p ? {
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

  setAsMainImage(productImageId: number): void {
    const current = this.productModel()!.productImages.find(i => i.productImageId === productImageId);
    if (current?.isMainImage) return;

    this.isSettingMain.set(productImageId);
    this.imageError.set(null);

    this.vendorProductService.makeImageDefault(productImageId).subscribe({
      next: () => {
        this.imageSuccess.set('Default Product Image Changed Successfully');

        this.productModel.update(p => p ? {
          ...p,
          productImages: p.productImages.map(i => ({
            ...i,
            isMainImage: i.productImageId === productImageId
          }))
        } : p);
        this.isSettingMain.set(null);
      },
      error: (error) => {
        if (error.error?.message) {
          this.imageError.set(error.error.message);
        } else if (error.status === 0) {
          this.imageError.set('Unable to set main image. Check your internet connection.');
        } else {
          this.imageError.set('Failed to set main image.');
        }
        this.isSettingMain.set(null);
      }
    });
  }

  sortedProductImages = computed(() => {
    const imgs = this.productModel()?.productImages ?? [];
    return [...imgs].sort((a, b) => {
      if (a.isMainImage !== b.isMainImage) return a.isMainImage ? -1 : 1;
      return a.displayOrder - b.displayOrder;
    });
  });



  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);

  loadCategories(): void {
    this.vendorProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
      },
      error: (error) => {
        if (error.status === 0) {
          this.errorMessage.set(
            'Unable to load categories. Check your internet connection.'
          );
        }
        else {
          this.errorMessage.set(
            'Failed to load product categories.'
          );
        }
      }
    });
  }

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;

    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);
    this.subCategories.set([]);
    if (id) {
      this.vendorProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res.items ?? res),
        error: (error) => {
          if (error.status === 0) {
            this.errorMessage.set(
              'Unable to load subcategories. Check your internet connection.'
            );
          }
          else {
            this.errorMessage.set(
              'Failed to load product subcategories.'
            );
          }
        }
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
  }

  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteProductModel = signal(new AdminDeleteProductModel());
  selectedProductId = signal<number | null>(null);


  reviewProductModel = signal(new ReviewProductModel());
  viewProduct(productId: number) {
    this.router.navigate(['/vendor/products', productId]);
  }

  progress = signal(false);
  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remarks, { message: "Enter The Remarks" });
    maxLength(path.remarks, 150, { message: "Maximum 100 characters" });
  });

  approvalStatusOption = [
    { id: 2, label: 'Accept' },
    { id: 3, label: 'Reject' },
  ];

  openReviewPopup(productVariantId: number) {
    this.selectedProductIdForUpdate.set(productVariantId);
    this.reviewProductModel.set(new ReviewProductModel(productVariantId, null, ""));
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
    this.progress.set(true);

    const request = {
      productId: this.reviewProductModel().productId,
      approvalStatusId: Number(this.reviewProductModel().approvalStatusId),
      remarks: this.reviewProductModel().remarks
    };
    this.vendorProductService.reviewProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product reviewed successfully");
        setTimeout(() => {
          this.onCancelReview();
          this.successMessage.set(null);
          this.loadData();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);
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
    this.reviewProductModel.set(new ReviewProductModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }


  showReviewPopup = signal(false);
  showUpdatePopup = signal(false);
  showUpdateRejectedPopup = signal(false);



  selectedProductIdForDelete = signal<number | null>(null);
  selectedProductIdForUpdate = signal<number | null>(null);
  selectedProductForUpdateRejected = signal<VendorProductModel | null>(null);

  openDeletePopup(productId: number) {
    this.selectedProductIdForDelete.set(productId);
    this.showActivatePopup.set(true);
  }
  closeDeletePopup() {
    this.showActivatePopup.set(false);
    this.selectedProductIdForDelete.set(null);
  }

  openUpdatePopup(productId: number) {
    this.selectedProductIdForUpdate.set(productId);
    this.showUpdatePopup.set(true);
  }
  closeUpdatePopup() {
    this.showUpdatePopup.set(false);
    this.selectedProductIdForUpdate.set(null);
  }

  openUpdateRejectedPopup(product: VendorProductModel) {
    this.loadCategories();
    this.selectedProductForUpdateRejected.set(product);
    this.showUpdateRejectedPopup.set(true);
  }
  closeUpdateRejectedPopup() {
    this.showUpdateRejectedPopup.set(false);
    this.selectedProductForUpdateRejected.set(null);
  }

  goBack(): void {
    this.router.navigate(['/vendor/products/list']);
  }

  viewVariant(id: number): void {
    this.router.navigate(['vendor/products/variant', id]);
  }
}
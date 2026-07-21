import { Component, signal } from '@angular/core';
import { AdminProductService } from '../../../services/admin-product.Service';
import { ProductModel } from '../../../models/product/product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ReviewProductModel } from '../../../models/product/review-product.model';
import { form, maxLength, required } from '@angular/forms/signals';
import { AdminDeleteProductModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { DeleteProductComponent } from '../../../vendor/vendor-product/delete-product-componentd/delete-product-componentd';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';
import { DeletePopupComponents } from '../../../shared-components/delete-popup-components/delete-popup-components';
import { ProductReviews } from '../../../users/user-product/product-reviews/product-reviews';
import { ProductHistoryComponent } from '../product-history-component/product-history-component';

@Component({
  selector: 'app-admin-detail-product',
  imports: [DatePipe, DecimalPipe, CommonModule, DeletePopupComponents, ReviewPopupComponent, ProductReviews, ProductHistoryComponent],
  templateUrl: './admin-detail-product.html',
  styleUrl: './admin-detail-product.css',
})
export class AdminDetailProduct {
  productModel = signal<ProductModel | null>(null);
  currentImageIndex = signal(0);


  constructor(private adminProductService: AdminProductService, private route: ActivatedRoute, private router: Router) {

  }
  ngOnInit(): void {
    const productId = Number(this.route.snapshot.paramMap.get('id'));

    if (productId) {
      this.loadProductDetails(productId);
    }
  }

  tableLoading = signal(false);
  loadProductDetails(productId: number) {
    this.tableLoading.set(true);
    this.adminProductService.getProductDetails(productId).subscribe({
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
  prevImage() {
    const total = this.productModel()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i - 1 + total) % total);
  }

  nextImage() {
    const total = this.productModel()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i + 1) % total);
  }

  goBack(): void {
    this.router.navigate(['/admin/products/list']);
  }

  showDeletePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteProductModel = signal(new AdminDeleteProductModel());
  progress = signal(false);

  deleteForm = form(this.deleteProductModel, (path) => {
    required(path.remark, { message: "Enter The Remarks" });
  })

  openDeletePopup(productId: number) {
    this.deleteProductModel.update(model => ({
      ...model,
      productId: productId,
      remark: ''
    }));
    this.showDeletePopup.set(true);
  }

  onConfirmDelete() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

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
      productId: this.deleteProductModel().productId,
      remark: this.deleteProductModel().remark
    };
    const id = this.productModel()?.productId;
    if (id == null) {
      return;
    }
    this.adminProductService.deleteProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product deleted successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.onCancelDelete();
          this.successMessage.set(null);
          this.loadProductDetails(id);
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

  onCancelDelete() {
    this.showDeletePopup.set(false);
    this.deleteForm().reset();
    this.deleteProductModel.set(new AdminDeleteProductModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  showReviewPopup = signal(false);
  reviewProductModel = signal(new ReviewProductModel());
  approvalStatusOption = [
    { id: 4, label: 'Accept' },
    { id: 5, label: 'Reject' },
  ];

  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remarks, { message: "Enter The Remarks" });
    maxLength(path.remarks, 150, { message: "Maximum 100 characters" });
  });

  openReviewPopup(productId: number) {
    this.reviewProductModel.update(model => ({
      ...model,
      productId: productId,
      approvalStatusId: null,
      remark: ''
    }));
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
    const id = this.productModel()?.productId;
    if (id == null) {
      return;
    }
    this.adminProductService.reviewProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product reviewed successfully");
        setTimeout(() => {
          this.onCancelReview();
          this.successMessage.set(null);
          this.loadProductDetails(id);
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

  viewProduct(productId: number) {
    this.router.navigate(['/admin/product-variant-details', productId]);
  }
}

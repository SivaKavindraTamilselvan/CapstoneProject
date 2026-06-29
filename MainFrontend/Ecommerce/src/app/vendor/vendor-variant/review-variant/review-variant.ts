import { Component, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductVariantFilter } from '../../../models/vendor/vendor-product/filter/vendor.varaint.filter';
import { UserProductCategoryModel } from '../../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../../models/user/product-category/user-sub-category.model';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { UserProductService } from '../../../services/user-product.Service';
import { ReviewProductVariantModel } from '../../../models/product/review-variant.model';
import { form, FormField, pattern, required } from '@angular/forms/signals';
import { DecimalPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-review-variant',
  imports: [DecimalPipe,FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './review-variant.html',
  styleUrl: './review-variant.css',
})
export class ReviewVariant {
  categories = signal<UserProductCategoryModel[]>([]);
  subCategories = signal<UserSubProductCategoryModel[]>([]);
  variant = signal<PagedResponse<VendorProductVariantModel> | null>(null);
  sku = signal<string>('');
  productId = signal<number | null>(null);
  searchTearm = signal<string>('');
  categoryId = signal<number | null>(null);
  subcategoryId = signal<number | null>(null);
  statusId = signal<number | null>(null);
  approvalstatusId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  minimuQuantityPerUser = signal<number | null>(null);
  addedByVendorUserId = signal<number | null>(null);
  minReservedQuantity = signal<number | null>(null);
  minAvailableQuantity = signal<number | null>(null);
  maxAvailableQuantity = signal<number | null>(null);
  maxReservedQuantity = signal<number | null>(null);
  mainProductSubCategoryAttributeId = signal<number | null>(null);
  isAvailableForSale = signal<boolean | null>(null);
  hasIssues = signal<boolean | null>(null);
  isReturn = signal<boolean | null>(null);
  isExchange = signal<boolean | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.variant()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  reviewProductModel = signal(new ReviewProductVariantModel());
  showActivatePopup = signal(false);


  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  selectedProductId = signal<number | null>(null);


  constructor(private route: Router, private vendorProductService: VendorProductService, private userProductService: UserProductService) {

  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadProductVariant();
  }

  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  loadProductVariant(): void {
    this.vendorProductService.getProductVariant(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.variant.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
  private buildFilter(): VendorProductVariantFilter {
    return {
      sku: this.sku(),
      productId: this.productId(),
      searchTerm: this.searchTearm() || null,
      categoryId: this.categoryId(),
      subCategoryId: this.subcategoryId(),
      statusId: this.statusId(),
      approvalStatusId: this.approvalstatusId(),
      addedByVendorUserId: this.addedByVendorUserId(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
      hasIssues: this.hasIssues(),
      minimuQuantityPerUser: this.minimuQuantityPerUser(),
      isAvailableForSale: this.isAvailableForSale(),
      minAvailableQuantity: this.minAvailableQuantity(),
      maxAvailableQuantity: this.maxAvailableQuantity(),
      minReservedQuantity: this.minReservedQuantity(),
      maxReservedQuantity: this.maxReservedQuantity(),
      mainProductSubCategoryAttributeId: this.mainProductSubCategoryAttributeId(),
      isExchange: this.isExchange(),
      isReturn: this.isReturn(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }

  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' },
    { id: 6, label: 'Deleted By Admin' },
  ];

  statusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily_Not_Available' },
    { id: 4, label: 'Archived' },
  ];

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadProductVariant();
    this.closeFilterPanel();
  }
  resetFilters(): void {
    this.sku.set('');
    this.productId.set(null);
    this.searchTearm.set('');
    this.categoryId.set(null);
    this.subcategoryId.set(null);
    this.statusId.set(null);
    this.approvalstatusId.set(null);
    this.addedByVendorUserId.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.hasIssues.set(null);
    this.minimuQuantityPerUser.set(null);
    this.isAvailableForSale.set(null);
    this.minAvailableQuantity.set(null);
    this.maxAvailableQuantity.set(null);
    this.minReservedQuantity.set(null);
    this.maxReservedQuantity.set(null);
    this.mainProductSubCategoryAttributeId.set(null);
    this.isExchange.set(true);
    this.isReturn.set(true);
    this.pageNumber.set(1);
    this.loadProductVariant();
    this.closeFilterPanel();
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadProductVariant();
  }

  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }

  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadProductVariant();
  }

  onSKUInput(event: Event): void {
    this.sku.set((event.target as HTMLInputElement).value);
  }

  onSearchTermInput(event: Event): void {
    this.searchTearm.set((event.target as HTMLInputElement).value);
  }

  onProductIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.productId.set(v ? Number(v) : null);
  }

  onCategoryIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.categoryId.set(v ? Number(v) : null);
  }

  onSubcategoryIdInput(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.subcategoryId.set(v ? Number(v) : null);
  }

  onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minPrice.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxPrice.set(v ? Number(v) : null);
  }

  onMinAvailableQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minAvailableQuantity.set(v ? Number(v) : null);
  }

  onMaxAvailableQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxAvailableQuantity.set(v ? Number(v) : null);
  }

  onMinReservedQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minReservedQuantity.set(v ? Number(v) : null);
  }

  onMaxReservedQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxReservedQuantity.set(v ? Number(v) : null);
  }

  onMinimumQuantityPerUserInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minimuQuantityPerUser.set(v ? Number(v) : null);
  }

  onAddedByVendorUserIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.addedByVendorUserId.set(v ? Number(v) : null);
  }

  onMainAttributeIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.mainProductSubCategoryAttributeId.set(v ? Number(v) : null);
  }

  onHasIssuesChange(event: Event): void {
    this.hasIssues.set((event.target as HTMLInputElement).checked || null);
  }

  onAvailableForSaleChange(event: Event): void {
    this.isAvailableForSale.set((event.target as HTMLInputElement).checked || null);
  }

  onStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.statusId.set(v ? Number(v) : null);
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.approvalstatusId.set(v ? Number(v) : null);
  }

  onIsReturnChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isReturn.set(checked ? true : null);
  }

  onIsExchangeChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isExchange.set(checked ? true : null);
  }

  loadCategories(): void {
    this.userProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res);
        console.log(this.categories);
      },
      error: (err) => console.log(err)
    });
  }

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.categoryId.set(id);
    this.subcategoryId.set(null);         // reset subcategory when category changes
    this.subCategories.set([]);
    if (id) {
      this.userProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res),
        error: (err) => console.log(err)
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.subcategoryId.set(v ? Number(v) : null);
  }
  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    pattern(path.approvalStatusId, /^[23]$/, { message: "Select valid approval status" })
  });

  openReviewPopup(productId: number) {
    this.selectedProductId.set(productId);

    this.reviewProductModel.set(
      new ReviewProductVariantModel(productId, "", "")
    );

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedProductId.set(null);
    this.reviewProductModel.set(new ReviewProductVariantModel());
    this.errorMessage.set(null);
  }
  handleReview() {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    if (this.reviewForm().invalid()) {
      this.errorMessage.set("Enter proper details");
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
          this.closePopup();
          this.successMessage.set(null);
          this.loadProductVariant();
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }
}

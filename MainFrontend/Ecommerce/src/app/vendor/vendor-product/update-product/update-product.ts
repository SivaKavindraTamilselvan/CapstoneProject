import { Component, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductFilter } from '../../../models/vendor/vendor-product/filter/vendor-product.filter';
import { UpdateProductStatus } from '../../../models/vendor/vendor-product/add-model/update-product-status.model';
import { form, required, min, max, FormField } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UpdateRejectedProductModel } from '../../../models/vendor/vendor-product/add-model/update-rejected-product.model';

@Component({
  selector: 'app-update-product',
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './update-product.html',
  styleUrl: './update-product.css',
})
export class UpdateProduct {
  products = signal<PagedResponse<VendorProductModel> | null>(null);

  searchTerm = signal<string>('');
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productApprovalStatusId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);
  productName = signal<string>('');
  addedByVendorUserId = signal<number | null>(null);
  minAvailableQuantity = signal<number | null>(null);
  maxAvailableQuantity = signal<number | null>(null);
  minReservedQuantity = signal<number | null>(null);
  maxReservedQuantity = signal<number | null>(null);
  mainProductSubCategoryAttributeId = signal<number | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  filterPanelOpen = signal<boolean>(false);

  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  selectedProductId = signal<number | null>(null);

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  reviewProductModel = signal(new UpdateProductStatus());


  updateForm = form(this.reviewProductModel, (path) => {
    required(path.productId, { message: "Enter The Approval Status" });
    required(path.productStatusId, { message: 'Enter The Approval Status' });
    min(path.productStatusId, 1, { message: 'Select valid approval status' });
    max(path.productStatusId, 4, { message: 'Select valid approval status' });
  });

  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  totalPages = computed(() => this.products()?.totalPages ?? 1);

  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' },
    { id: 6, label: 'Deleted By Admin' },
  ];

  productStatusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily Not Available' },
    { id: 4, label: 'Archived' },
  ];

  constructor(private route: Router, private vendorProductService: VendorProductService) { }

  ngOnInit(): void {
    this.loadProduct();
  }

  private buildFilter(): VendorProductFilter {
    return {
      productName: this.productName() || null,
      productCategoryId: this.productCategoryId(),
      productSubCategoryId: this.productSubCategoryId(),
      productApprovalStatusId: this.productApprovalStatusId(),
      productStatusId: this.productStatusId(),
      addedByVendorUserId: this.addedByVendorUserId(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
      searchTerm: this.searchTerm() || null,
      hasIssues: this.hasIssues(),
      isAvailableForSale: this.isAvailableForSale(),
      minAvailableQuantity: this.minAvailableQuantity(),
      maxAvailableQuantity: this.maxAvailableQuantity(),
      minReservedQuantity: this.minReservedQuantity(),
      maxReservedQuantity: this.maxReservedQuantity(),
      mainProductSubCategoryAttributeId: this.mainProductSubCategoryAttributeId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }

  openReviewPopup(productId: number) {
    this.selectedProductId.set(productId);

    this.reviewProductModel.set(
      new UpdateProductStatus(productId, 0)
    );

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedProductId.set(null);
    this.reviewProductModel.set(new UpdateProductStatus());
    this.errorMessage.set(null);
  }
  handleReview() {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    if (this.updateForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }
    const request = {
      productId: this.reviewProductModel().productId,
      productStatusId: Number(this.reviewProductModel().productStatusId),
    };
    this.vendorProductService.updateProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Vendor reviewed successfully");
        setTimeout(() => {
          this.closePopup();
          this.successMessage.set(null);
          this.loadProduct();
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

  loadProduct(): void {
    this.vendorProductService.getProduct(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.products.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.productName.set('');
    this.searchTerm.set('');
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);
    this.productApprovalStatusId.set(null);
    this.productStatusId.set(null);
    this.addedByVendorUserId.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.hasIssues.set(null);
    this.isAvailableForSale.set(null);
    this.minAvailableQuantity.set(null);
    this.maxAvailableQuantity.set(null);
    this.minReservedQuantity.set(null);
    this.maxReservedQuantity.set(null);
    this.mainProductSubCategoryAttributeId.set(null);
    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadProduct();
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
    this.loadProduct();
  }

  onSearchTermInput(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productApprovalStatusId.set(v ? Number(v) : null);
  }

  onProductStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productStatusId.set(v ? Number(v) : null);
  }

  onMinAvailableInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minAvailableQuantity.set(v ? Number(v) : null);
  }

  onMaxAvailableInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxAvailableQuantity.set(v ? Number(v) : null);
  }

  onMinReservedInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minReservedQuantity.set(v ? Number(v) : null);
  }

  onMaxReservedInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxReservedQuantity.set(v ? Number(v) : null);
  }

  onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minPrice.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxPrice.set(v ? Number(v) : null);
  }

  onHasIssuesChange(event: Event): void {
    this.hasIssues.set((event.target as HTMLInputElement).checked || null);
  }

  onAvailableForSaleChange(event: Event): void {
    this.isAvailableForSale.set((event.target as HTMLInputElement).checked || null);
  }

  loadCategories(): void {
    this.vendorProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
        console.log(this.categories);
      },
      error: (err) => console.log(err)
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
        error: (err) => console.log(err)
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
  }
  onUpdateStatusChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);

    this.reviewProductModel.update((model) => ({
      ...model,
      productStatusId: value
    }));
  }
}


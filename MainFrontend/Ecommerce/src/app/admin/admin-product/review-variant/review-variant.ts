import { Component, computed, signal } from '@angular/core';
import { UserProductCategoryModel } from '../../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../../models/user/product-category/user-sub-category.model';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { ReviewProductVariantModel } from '../../../models/product/review-variant.model';
import { Router } from '@angular/router';
import { UserProductService } from '../../../services/user-product.Service';
import { AdminProductVariantFilter } from '../../../models/admin/admin-product/filter/admin-variant.filter';
import { form, FormField, pattern, required } from '@angular/forms/signals';
import { AdminProductService } from '../../../services/admin-product.Service';
import { DecimalPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { ProductVariantModel } from '../../../models/product/product-variant.model';

@Component({
  selector: 'app-review-variant',
  imports: [DecimalPipe,FormsModule,ReactiveFormsModule,FormField],
  templateUrl: './review-variant.html',
  styleUrl: './review-variant.css',
})
export class ReviewVariant {
  actions: TableAction<ProductVariantModel>[] = [
        {
          label: 'View',
          color: 'green',
          action: 'view'
        },
        {
          label: 'Review',
          color: 'blue',
          action: 'review'
        }
      ];
      columns: Column[] = [
        {
          key: 'productId',
          header: 'ID'
        },
        {
          key: 'productName',
          header: 'Name'
        },
        {
          key: 'productCategoryName',
          header: 'Category'
        },
        {
          key: 'productSubCategoryName',
          header: 'SubCategory'
        },
        {
          key: 'vendorName',
          header: 'Vendor'
        },
        {
          key: 'productApprovalStatus',
          header: 'Approval'
        },
        {
          key: 'productStatus',
          header: 'Status'
        },
      ];
    
      mobileColumns: Column[] = [
         {
          key: 'productName',
          header: 'Name'
        },
        {
          key: 'productCategoryName',
          header: 'Category'
        },
        {
          key: 'productSubCategoryName',
          header: 'Sub Category'
        },
        {
          key: 'vendorName',
          header: 'Vendor'
        },
        {
          key: 'productApprovalStatus',
          header: 'Approval'
        },
        {
          key: 'productStatus',
          header: 'Status'
        },
        
      ];
      handleAction(event: { type: string; row:ProductVariantModel }) {
          switch (event.type) {
            case 'view':
              this.viewProduct(event.row.productVariantId);
              break;
            case 'delete':
              this.openReviewPopup(event.row.productVariantId);
              break;
          }
        }
  categories = signal<UserProductCategoryModel[]>([]);
  subCategories = signal<UserSubProductCategoryModel[]>([]);
  variant = signal<PagedResponse<VendorProductVariantModel> | null>(null);
  sku = signal<string>('');
  productId = signal<number | null>(null);
  vendorId = signal<number | null>(null);
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

  filtererrorMessage = signal<string | null>(null);
  filterapplied = signal(false);


  reviewProductModel = signal(new ReviewProductVariantModel());
  showActivatePopup = signal(false);


  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  selectedProductId = signal<number | null>(null);


  constructor(private route: Router, private adminProductService: AdminProductService, private userProductService: UserProductService) {

  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadProductVariant();
  }

   toggleFilterPanel(): void {
    const wasOpen = this.filterPanelOpen();
    this.filterPanelOpen.update((open) => !open);
    if (wasOpen && !this.filterapplied()) {
      this.resetFilters();
    }
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  loadProductVariant(): void {
    this.adminProductService.getProductVariant(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.variant.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
  private buildFilter(): AdminProductVariantFilter {
    return {
      sku: this.sku(),
      VendorId: this.vendorId(),
      productId: this.productId(),
      searchTerm: this.searchTearm(),
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
    if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadProductVariant();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.filtererrorMessage.set("");
    this.filterapplied.set(false);
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
   onPageSizeChanged(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadProductVariant();
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
    this.categoryId.set(id);
    this.subcategoryId.set(null);         // reset subcategory when category changes
    this.subCategories.set([]);
    if (id) {
      this.userProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res),
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
    this.subcategoryId.set(v ? Number(v) : null);
  }
  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remarks, { message: "Enter The Remarks" });
  });

  openReviewPopup(productId: number) {
    this.selectedProductId.set(productId);

    this.reviewProductModel.set(
      new ReviewProductVariantModel(productId, null, "")
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
      remarks: this.reviewProductModel().remarks
    };
    this.adminProductService.reviewProductVariant(request).subscribe({
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
  viewProduct(productId: number) {
    this.route.navigate(['/admin/product-details', productId]);
  }
}


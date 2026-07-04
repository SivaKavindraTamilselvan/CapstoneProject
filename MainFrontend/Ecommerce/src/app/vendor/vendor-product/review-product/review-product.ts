import { Component, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductFilter } from '../../../models/vendor/vendor-product/filter/vendor-product.filter';
import { form, FormField, pattern, required } from '@angular/forms/signals';
import { ReviewProductModel } from '../../../models/product/review-product.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { ProductModel } from '../../../models/product/product.model';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';

@Component({
  selector: 'app-review-product',
  imports: [FormField, ReactiveFormsModule, FormsModule, MobileCardComponent, DataTableComponent, FilterComponent, PaginationComponent],
  templateUrl: './review-product.html',
  styleUrl: './review-product.css',
})
export class ReviewProduct {
  actions: TableAction[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    },
    {
      label: 'Review',
      color: 'gray',
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
      key: 'productApprovalStatus',
      header: 'Approval',
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
      key: 'productApprovalStatus',
      header: 'Approval',
    },
    {
      key: 'productStatus',
      header: 'Status'
    },
  ];
  handleAction(event: { type: string; row: VendorProductModel }) {
    switch (event.type) {
      case 'view':
        this.viewProduct(event.row.productId);
        break;
      case 'review':
        this.openReviewPopup(event.row.productId);
        break;
    }
  }
  products = signal<PagedResponse<ProductModel> | null>(null);
  showActivatePopup = signal(false);
  selectedProductId = signal<number | null>(null);
  reviewProductModel = signal(new ReviewProductModel());
  searchTerm = signal<string>('');
  vendorId = signal<number | null>(null);
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);

  filterPanelOpen = signal<boolean>(false);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  filtererrorMessage = signal<string | null>(null);
  reviewerrorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);

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
    this.loadCategories();
  }

  private buildFilter(): VendorProductFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm() || null,
      productName: null,
      productCategoryId: this.productCategoryId(),
      productSubCategoryId: this.productSubCategoryId(),
      productStatusId: this.productStatusId(),
      productApprovalStatusId: null,
      addedByVendorUserId: null,
      minPrice: null,
      maxPrice: null,

      minAvailableQuantity: null,
      maxAvailableQuantity: null,

      minReservedQuantity: null,
      maxReservedQuantity: null,

      mainProductSubCategoryAttributeId: null,

      hasIssues: this.hasIssues(),
      isAvailableForSale: null
    };
  }

  loadProduct(): void {
    this.vendorProductService.getReviewProducts(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.products.set(response);
      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.products.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
        else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.errorMessage);
        }
      },
    });
  }

  applyFilters(): void {
    if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  resetFilters(): void {
     this.filtererrorMessage.set("");
    this.filterapplied.set(false);
    this.searchTerm.set('');
    this.vendorId.set(null);
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);
    this.productStatusId.set(null);
    this.hasIssues.set(null);
    this.pageNumber.set(1);
    this.loadProduct();
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

  onPageSizeChanged(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadProduct();
  }

  onSearchTermInput(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  onVendorIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.vendorId.set(v ? Number(v) : null);
  }

  onStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productStatusId.set(v ? Number(v) : null);
  }

  onHasIssuesChange(event: Event): void {
    this.hasIssues.set((event.target as HTMLInputElement).checked || null);
  }

  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    pattern(path.approvalStatusId, /^[23]$/, { message: "Select valid approval status" })
  });

  openReviewPopup(productId: number) {
    this.selectedProductId.set(productId);

    this.reviewProductModel.set(
      new ReviewProductModel(productId, "", "")
    );

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedProductId.set(null);
    this.reviewProductModel.set(new ReviewProductModel());
    this.reviewerrorMessage.set(null);
    this.reviewForm().reset();
  }
  handleReview() {
    this.reviewerrorMessage.set(null);
    this.successMessage.set(null);
    if (this.reviewForm().invalid()) {
      this.reviewerrorMessage.set("Enter proper details");
      return;
    }
    const request = {
      productId: this.reviewProductModel().productId,
      approvalStatusId: Number(this.reviewProductModel().approvalStatusId),
      remark: this.reviewProductModel().remark
    };
    this.vendorProductService.reviewProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product reviewed successfully");
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

          this.reviewerrorMessage.set(messages);
        }
        else {
          this.reviewerrorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }
  loadCategories(): void {
    this.errorMessage.set(null);
    this.vendorProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
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
    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);
    this.subCategories.set([]);
    if (id) {
      this.errorMessage.set(null);
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
  viewProduct(productId: number) {
    this.route.navigate(['/vendor/products', productId]);
  }
}


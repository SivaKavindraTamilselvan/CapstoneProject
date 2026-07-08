import { Component, OnInit, computed, effect, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { ProductModel } from '../../../models/product/product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminProductService } from '../../../services/admin-product.Service';
import { AdminProductFilter } from '../../../models/admin/admin-product/filter/admin-product.filter';
import { AdminDeleteProductModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { form, FormField, min, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { ReviewProductModel } from '../../../models/product/review-product.model';

@Component({
  selector: 'app-admin-product',
  imports: [FormField, FormsModule, ReactiveFormsModule, PaginationComponent, FilterComponent, MobileCardComponent, DataTableComponent],
  templateUrl: './admin-product.html',
  styleUrl: './admin-product.css',
})
export class AdminProduct extends BasePage {
  actions: TableAction<ProductModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view',
    },
    {
      label: 'Delete',
      color: 'red',
      action: 'delete',
      visible: vendor => vendor.productApprovalStatus != "Deleted_By_Admin" && vendor.productApprovalStatus != "Pending"

    },
    {
      label: 'Review',
      color: 'gray',
      action: 'review',
      visible: vendor => vendor.productApprovalStatus == "Pending"
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

  handleAction(event: { type: string; row: ProductModel }) {
    switch (event.type) {
      case 'view':
        this.viewProduct(event.row.productId);
        break;
      case 'delete':
        this.openDeletePopup(event.row.productId);
        break;
      case 'review':
        this.openReviewPopup(event.row.productId);
        break;
    }
  }

  products = signal<PagedResponse<ProductModel> | null>(null);

  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productApprovalStatusId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);

  showDeletePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteProductModel = signal(new AdminDeleteProductModel());
  selectedProductId = signal<number | null>(null);

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  totalPages = computed(() => this.products()?.totalPages ?? 1);

  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' }
  ];

  productStatusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily Not Available' },
    { id: 4, label: 'Archived' },
  ];

  adminProductFilter = signal(new AdminProductFilter());

  constructor(private route: Router, private adminProductService: AdminProductService, private router: ActivatedRoute) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  protected loadData(): void {
    this.loadProduct();
  }

  clearFilterValues(): void {
    this.productStatusId.set(null);
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);;
    this.hasIssues.set(null);
    this.draftstatus.set(null);
    this.isAvailableForSale.set(null);
    this.subCategories.set([]);
    this.adminProductFilter.set(new AdminProductFilter());
    this.adminProductFilter.update(model => ({
      ...model,
      productStatusId: null,
      hasIssues: null,
      isAvailableForSale: null,
      productCategoryId: null,
      productSubCategoryId: null
    }));
  }

  draftstatus = signal<number | null>(null);
  status = signal<number | null>(null);
  deleted = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.status.set(data['status']);
      this.deleted.set(data['deleted']);
      this.pageTitle.set(data['title']);
      this.loadProduct();
      this.loadCategories();
    });
  }

  private buildFilters() {
    this.adminProductFilter.update(filter => ({
      ...filter,
      productApprovalStatusId: this.status() == null && this.deleted() == false ? this.draftstatus() : this.status(),
      includeIsDeleted: this.deleted(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: filter.searchTerm.trim().toLowerCase(),
    }));
  }

  filterForm = form(this.adminProductFilter, (path) => {
    min(path.vendorId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.addedId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productCategoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productSubCategoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productApprovalStatusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productStatusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.minPrice, 0, { message: 'Minimum price cannot be negative or 0.' });
    min(path.maxPrice, 0, { message: 'Maximum price cannot be negative or 0.' });
    min(path.minAvailableQuantity, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.maxAvailableQuantity, 0, { message: 'Maximum available quantity cannot be negative.' });
    min(path.minReservedQuantity, 0, { message: 'Minimum reserved quantity cannot be negative.' });
    min(path.maxReservedQuantity, 0, { message: 'Maximum reserved quantity cannot be negative.' });
  });

  loadProduct(): void {
    this.buildFilters();
    console.log(this.adminProductFilter().includeIsDeleted);
    this.adminProductService.getProducts(this.adminProductFilter()).subscribe({
      next: (response: any) => {
        this.products.set(response);
      },
      error: (error) => {
        console.log(error);

      },
    });
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productApprovalStatusId.set(v ? Number(v) : null);
    this.draftstatus.set(v ? Number(v) : null);
  }

  onProductStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productStatusId.set(v ? Number(v) : null);
    this.adminProductFilter.update(model => ({
      ...model,
      productStatusId: v ? Number(v) : null
    }));
  }

  onHasIssuesChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.hasIssues.set(checked);
    this.adminProductFilter.update(model => ({
      ...model,
      hasIssues: checked
    }));
  }

  onAvailableForSaleChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;

    this.isAvailableForSale.set(checked ? true : null);

    this.adminProductFilter.update(model => ({
      ...model,
      productStatusId: checked ? 1 : null
    }));
  }

  deleteForm = form(this.deleteProductModel, (path) => {
    required(path.remark, { message: "Enter The Remarks" });
  })

  handleDelete() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.deleteForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    const request = {
      productId: this.deleteProductModel().productId,
      remark: this.deleteProductModel().remark
    };

    this.adminProductService.deleteProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product deleted successfully. Closing in 3 seconds...");
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

  openDeletePopup(productId: number) {
    this.selectedProductId.set(productId);

    this.deleteProductModel.update(model => ({
      ...model,
      productId: productId,
      remark: ''
    }));

    this.showDeletePopup.set(true);
  }
  reviewProductModel = signal(new ReviewProductModel());

  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    pattern(path.approvalStatusId, /^[45]$/, { message: "Select valid approval status" })
  });


  openReviewPopup(productId: number) {
    this.selectedProductId.set(productId);

    this.reviewProductModel.set(
      new ReviewProductModel(productId, "", "")
    );

    this.showPopup.set(true);
  }

  closeDeletePopup() {
    this.showDeletePopup.set(false);
    this.selectedProductId.set(null);
    this.reviewProductModel.set(new ReviewProductModel());
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
      productId: this.reviewProductModel().productId,
      approvalStatusId: Number(this.reviewProductModel().approvalStatusId),
      remark: this.reviewProductModel().remark
    };
    this.adminProductService.reviewProduct(request).subscribe({
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


  loadCategories(): void {
    this.adminProductService.getProductCategory().subscribe({
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

    this.adminProductFilter.update(model => ({
      ...model,
      productCategoryId: id,
      productSubCategoryId: null
    }));

    this.subCategories.set([]);
    if (id) {
      this.adminProductService.getSubCategory(id).subscribe({
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
    this.adminProductFilter.update(model => ({
      ...model,
      productSubCategoryId: v ? Number(v) : null
    }));
  }
  viewProduct(productId: number) {
    this.route.navigate(['/admin/product-details', productId]);
  }
}
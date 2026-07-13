import { Component, computed, effect, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminProductService } from '../../../services/admin-product.Service';
import { VendorProductFilter } from '../../../models/vendor/vendor-product/filter/vendor-product.filter';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { AdminDeleteProductModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { form, FormField, max, maxLength, min, pattern, required } from '@angular/forms/signals';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { UpdateProductStatus } from '../../../models/vendor/vendor-product/add-model/update-product-status.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { ReviewProductModel } from '../../../models/product/review-product.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UpdateRejectedProductModel } from '../../../models/vendor/vendor-product/add-model/update-rejected-product.model';
import { MappedAttributeFilter } from '../../../models/admin/admin-product-category/filter-models/mapped-attribute.filter';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';
import { AdminProductFilter } from '../../../models/admin/admin-product/filter/admin-product.filter';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { UpdateRejectedProductComponent } from '../update-rejected-product-component/update-rejected-product-component';
import { UpdateProductComponent } from '../update-product-component/update-product-component';
import { DeleteProductComponent } from '../delete-product-componentd/delete-product-componentd';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';

@Component({
  selector: 'app-product-list',
  imports: [PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent, FormField, ReactiveFormsModule, FormsModule, HeaderComponent, UpdateRejectedProductComponent, UpdateProductComponent, DeleteProductComponent, ReviewPopupComponent],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList extends BasePage {
  actions = computed<TableAction<VendorProductModel>[]>(() => {
    if (this.pageTitle() == 'Update Product') return [
      { label: 'View', color: 'green', action: 'view' },
      { label: 'Update', color: 'blue', action: 'update' }
    ];

    if (this.status() === 1) return [
      { label: 'View', color: 'green', action: 'view' },
      { label: 'Review', color: 'gray', action: 'review' }
    ];

    if (this.status() === 5) return [
      { label: 'View', color: 'green', action: 'view' },
      { label: 'Update', color: 'blue', action: 'update-rejected' }
    ];

    if (this.deleted()) return [
      { label: 'View', color: 'green', action: 'view' }
    ];

    return [
      { label: 'View', color: 'green', action: 'view' },
      { label: 'Delete', color: 'red', action: 'delete' }
    ];
  });

  columns: Column[] = [
    { key: 'productId', header: 'ID' },
    { key: 'productName', header: 'Name' },
    { key: 'productCategoryName', header: 'Category' },
    { key: 'productSubCategoryName', header: 'SubCategory' },
    { key: 'productApprovalStatus', header: 'Approval' },
    { key: 'productStatus', header: 'Status' }
  ];

  mobileColumns = [...this.columns];
  handleAction(event: { type: string; row: VendorProductModel }) {
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
      case 'update':
        this.openUpdatePopup(event.row.productId);
        break;
      case 'update-rejected':
        this.openUpdateRejectedPopup(event.row);
        break;
    }
  }


  products = signal<PagedResponse<VendorProductModel> | null>(null);

  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productApprovalStatusId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);

  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);


  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteProductModel = signal(new AdminDeleteProductModel());
  selectedProductId = signal<number | null>(null);

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  progress = signal(false);

  deleteerrorMessage = signal<string | null>(null);
  deletesuccessMessage = signal<string | null>(null);


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

  adminProductFilter = signal(new VendorProductFilter());

  constructor(private router: ActivatedRoute, private route: Router, private vendorProductService: VendorProductService) {
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
    this.adminProductFilter.set(new VendorProductFilter());
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
    min(path.addedByVendorUserId, 1, { message: 'ID cannot be negative or 0.' });
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
    this.vendorProductService.getProduct(this.adminProductFilter()).subscribe({
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
        else if (error.status === 0) {
          this.errorMessage.set(
            'Unable to connect to the server. Please check your internet connection.'
          );
        }
        else if (error.status >= 500) {
          this.errorMessage.set(
            'Something went wrong on the server. Please try again later.'
          );
        }
        else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        }
        else {
          this.errorMessage.set('Failed to load products.');
        }
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
      isAvailableForSale: checked
    }));
  }

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
    this.adminProductFilter.update(model => ({
      ...model,
      productCategoryId: id,
      productSubCategoryId: null
    }));

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
    this.adminProductFilter.update(model => ({
      ...model,
      productSubCategoryId: v ? Number(v) : null
    }));
  }

  viewProduct(productId: number) {
    this.route.navigate(['/vendor/products', productId]);
  }

  reviewProductModel = signal(new ReviewProductModel());
  
  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remarks, { message: "Enter The Remarks" });
    maxLength(path.remarks, 150, { message: "Maximum 100 characters" });
  });

  approvalStatusOption = [
    { id: 2, label: 'Accepted' },
    { id: 3, label: 'Rejected' },
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



  
  reviewerrorMessage = signal<string | null>(null);

  showReviewPopup = signal(false);
  showUpdatePopup = signal(false);
  showUpdateRejectedPopup = signal(false);

  closeReviewPopup() {
    this.showReviewPopup.set(false);
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
      remarks: this.reviewProductModel().remarks
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
    this.selectedProductForUpdateRejected.set(product);
    this.showUpdateRejectedPopup.set(true);
  }
  closeUpdateRejectedPopup() {
    this.showUpdateRejectedPopup.set(false);
    this.selectedProductForUpdateRejected.set(null);
  }
}

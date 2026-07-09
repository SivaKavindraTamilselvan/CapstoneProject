import { Component, computed, effect, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductVariantFilter } from '../../../models/vendor/vendor-product/filter/vendor.varaint.filter';
import { DecimalPipe } from '@angular/common';
import { UserProductService } from '../../../services/user-product.Service';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { ReviewProductModel } from '../../../models/product/review-product.model';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { UpdateRejectedProductComponent } from '../../vendor-product/update-rejected-product-component/update-rejected-product-component';
import { DeletePopupComponents } from '../../../shared-components/delete-popup-components/delete-popup-components';
import { ReviewProductVariantModel } from '../../../models/product/review-variant.model';
import { UpdateRejectVairantComponent } from '../update-reject-vairant-component/update-reject-vairant-component';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { DeleteProductComponent } from '../../vendor-product/delete-product-componentd/delete-product-componentd';
import { UpdateProductComponent } from '../../vendor-product/update-product-component/update-product-component';

@Component({
  selector: 'app-variant-list',
  imports: [DecimalPipe, FormField, ReactiveFormsModule, FormsModule, PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent, UpdateRejectVairantComponent,DeleteProductComponent, ReviewPopupComponent, HeaderComponent,UpdateProductComponent],
  templateUrl: './variant-list.html',
  styleUrl: './variant-list.css',
})
export class VariantList extends BasePage {

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);
  variant = signal<PagedResponse<VendorProductVariantModel> | null>(null);

  categoryId = signal<number | null>(null);
  subcategoryId = signal<number | null>(null);
  statusId = signal<number | null>(null);
  approvalstatusId = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);
  isReturn = signal<boolean | null>(null);
  isExchange = signal<boolean | null>(null);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  totalPages = computed(() => this.variant()?.totalPages ?? 1);

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

  vendorProductFilter = signal(new VendorProductVariantFilter());

  constructor(
    private router: ActivatedRoute,
    private route: Router,
    private vendorProductService: VendorProductService,
    private userProductService: UserProductService
  ) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  filterForm = form(this.vendorProductFilter, (path) => {
    min(path.productId, 1, { message: 'Product ID cannot be negative or 0.' });
    min(path.statusId, 1, { message: 'Status ID cannot be negative or 0.' });
    min(path.approvalStatusId, 1, { message: 'Approval Status ID cannot be negative or 0.' });
    min(path.categoryId, 1, { message: 'Category ID cannot be negative or 0.' });
    min(path.subCategoryId, 1, { message: 'Sub Category ID cannot be negative or 0.' });
    min(path.minimuQuantityPerUser, 1, { message: 'Minimum quantity per user cannot be negative or 0.' });
    min(path.addedByVendorUserId, 1, { message: 'Vendor User ID cannot be negative or 0.' });
    min(path.mainProductSubCategoryAttributeId, 1, { message: 'Main Attribute ID cannot be negative or 0.' });
    min(path.minPrice, 0, { message: 'Minimum price cannot be negative.' });
    min(path.maxPrice, 0, { message: 'Maximum price cannot be negative.' });
    min(path.minAvailableQuantity, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.maxAvailableQuantity, 0, { message: 'Maximum available quantity cannot be negative.' });
    min(path.minReservedQuantity, 0, { message: 'Minimum reserved quantity cannot be negative.' });
    min(path.maxReservedQuantity, 0, { message: 'Maximum reserved quantity cannot be negative.' });
  });

  protected loadData(): void {
    this.loadProductVariant();
  }

  draftstatus = signal<number | null>(null);
  status = signal<number | null>(null);
  deleted = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);
  update = signal<boolean | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.status.set(data['status']);
      this.deleted.set(data['deleted']);
      this.pageTitle.set(data['title']);
      this.update.set(data['update']);
      this.loadProductVariant();
      this.loadCategories();
    });
  }

  statusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' },
    { id: 6, label: 'Deleted By Admin' },
  ];


  private buildFilters() {
    this.vendorProductFilter.update(filter => ({
      ...filter,
      approvalStatusId: this.status() == null && this.deleted() == false ? this.draftstatus() : this.status(),
      includeIsDeleted: this.deleted(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: filter.searchTerm.trim().toLowerCase(),
    }));
  }

  loadProductVariant(): void {
    this.buildFilters();
    this.vendorProductService.getProductVariant(this.vendorProductFilter()).subscribe({
      next: (response: any) => {
        this.variant.set(response);
        console.log(response);
      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.variant.set({
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
          this.errorMessage.set('Failed to load variants.');
        }
      },
    });
  }

  clearFilterValues(): void {
    this.statusId.set(null);
    this.approvalstatusId.set(null);
    this.categoryId.set(null);
    this.subcategoryId.set(null);
    this.hasIssues.set(null);
    this.draftstatus.set(null);
    this.isAvailableForSale.set(null);
    this.isReturn.set(null);
    this.isExchange.set(null);
    this.subCategories.set([]);

    this.vendorProductFilter.set(new VendorProductVariantFilter());
    this.vendorProductFilter.update(model => ({
      ...model,
      statusId: null,
      approvalStatusId: null,
      categoryId: null,
      subCategoryId: null,
      hasIssues: null,
      isAvailableForSale: null,
      isReturn: null,
      isExchange: null
    }));
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.approvalstatusId.set(v ? Number(v) : null);
    this.draftstatus.set(v ? Number(v) : null);
  }

  onProductStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.statusId.set(v ? Number(v) : null);
    this.vendorProductFilter.update(model => ({
      ...model,
      statusId: v ? Number(v) : null
    }));
  }

  onHasIssuesChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.hasIssues.set(checked);
    this.vendorProductFilter.update(model => ({
      ...model,
      hasIssues: checked
    }));
  }

  onAvailableForSaleChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isAvailableForSale.set(checked ? true : null);
    this.vendorProductFilter.update(model => ({
      ...model,
      isAvailableForSale: checked
    }));
  }

  onIsReturnChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isReturn.set(checked ? true : null);
    this.vendorProductFilter.update(model => ({
      ...model,
      isReturn: checked ? true : null
    }));
  }

  onIsExchangeChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isExchange.set(checked ? true : null);
    this.vendorProductFilter.update(model => ({
      ...model,
      isExchange: checked ? true : null
    }));
  }

  loadCategories(): void {
    this.userProductService.getProductCategory().subscribe({
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
    this.vendorProductFilter.update(model => ({
      ...model,
      categoryId: id,
      subCategoryId: null
    }));

    this.categoryId.set(id);
    this.subcategoryId.set(null);
    this.subCategories.set([]);
    if (id) {
      this.userProductService.getSubCategory(id).subscribe({
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
    this.subcategoryId.set(v ? Number(v) : null);
    this.vendorProductFilter.update(model => ({
      ...model,
      subCategoryId: v ? Number(v) : null
    }));
  }

  actions = computed<TableAction<VendorProductVariantModel>[]>(() => {
    if (this.status() == null && this.update()==true) return [
      { label: 'View', color: 'green', action: 'view' },
      { label: 'Update', color: 'blue', action: 'update' }
    ];

    if (this.status() === 1) return [
      { label: 'View', color: 'green', action: 'view' },
      { label: 'Review', color: 'gray', action: 'review' }
    ];

    if (this.status() === 6) return [
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
    { key: 'productVariantId', header: 'Id' },
    { key: 'sku', header: 'SKU' },
    { key: 'productName', header: 'Name' },
    { key: 'productCategoryName', header: 'Category' },
    { key: 'productSubCategoryName', header: 'Sub Category' },
    { key: 'price', header: 'Price', formatter: value => `₹${Number(value).toLocaleString()}` },
    { key: 'availableQuantity', header: 'Stock' },
    { key: 'productVariantStatus', header: 'Variant Status' },
    { key: 'productVariantApprovalStatus', header: 'Approval' },
    { key: 'isAvailableForSale', header: 'For Sale', formatter: value => value ? 'Yes' : 'No' }
  ];
  mobileColumns = [...this.columns];

  handleAction(event: { type: string; row: VendorProductVariantModel }) {
    switch (event.type) {
      case 'view':
        this.viewProduct(event.row.productVariantId);
        break;
      case 'delete':
        this.openDeletePopup(event.row.productVariantId);
        break;
      case 'review':
        this.openReviewPopup(event.row.productVariantId);
        break;
      case 'update':
        this.openUpdatePopup(event.row.productVariantId);
        break;
      case 'update-rejected':
        this.openUpdateRejectedPopup(event.row);
        break;
    }
  }

  viewProduct(productVariantId: number) {
    this.route.navigate(['/vendor/products/variant', productVariantId]);
  }

  showReviewPopup = signal(false);
  reviewProductModel = signal(new ReviewProductVariantModel());
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
    this.progress.set(true);

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
          this.successMessage.set(null);
          this.loadProductVariant();
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
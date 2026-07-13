import { Component, computed, effect, signal } from '@angular/core';
import { UserProductCategoryModel } from '../../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../../models/user/product-category/user-sub-category.model';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { ActivatedRoute, Router } from '@angular/router';
import { UserProductService } from '../../../services/user-product.Service';
import { AdminProductVariantFilter } from '../../../models/admin/admin-product/filter/admin-variant.filter';
import { AdminProductService } from '../../../services/admin-product.Service';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { ProductVariantModel } from '../../../models/product/product-variant.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { combineLatest } from 'rxjs';
import { DeletePopupComponents } from '../../../shared-components/delete-popup-components/delete-popup-components';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';
import { AdminDeleteVariantModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { ReviewProductVariantModel } from '../../../models/product/review-variant.model';
import { AiValidationResult } from '../../../models/ai.model';

@Component({
  selector: 'app-variant-list',
  imports: [
    DataTableComponent,
    FilterComponent,
    MobileCardComponent,
    PaginationComponent,
    FormField,
    FormsModule,
    ReactiveFormsModule,
    DeletePopupComponents,
    ReviewPopupComponent
  ],
  templateUrl: './variant-list.html',
  styleUrl: './variant-list.css',
})
export class VariantList extends BasePage {

  variant = signal<PagedResponse<VendorProductVariantModel> | null>(null);

  selectedProductVariantId = signal<number | null>(null);

  categories = signal<UserProductCategoryModel[]>([]);
  subCategories = signal<UserSubProductCategoryModel[]>([]);

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

  adminVariantFilter = signal(new AdminProductVariantFilter());

  constructor(private route: Router, private adminProductService: AdminProductService, private userProductService: UserProductService, private router: ActivatedRoute) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  draftstatus = signal<number | null>(null);
  status = signal<number | null>(null);
  deleted = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);
  queryVendor = signal<number | null>(null);

  ngOnInit(): void {
    combineLatest([
      this.router.data,
      this.router.queryParams
    ]).subscribe(([data, params]) => {
      this.status.set(data['status']);
      this.deleted.set(data['deleted']);
      this.pageTitle.set(data['title']);
      this.queryVendor.set(
        params['vendorId'] ? Number(params['vendorId']) : null
      );
      this.loadProductVariant();

    });

    this.loadCategories();
  }

  protected loadData(): void {
    this.loadProductVariant();
  }

  loadProductVariant(): void {
    this.buildFilters();
    this.adminProductService.getProductVariant(this.adminVariantFilter()).subscribe({
      next: (response: any) => {
        this.variant.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productApprovalStatusId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);
  isReturn = signal<boolean | null>(null);
  isExchange = signal<boolean | null>(null);

  filterForm = form(this.adminVariantFilter, (path) => {
    min(path.VendorId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.categoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.subCategoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.statusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.approvalStatusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.minPrice, 0, { message: 'Minimum price cannot be negative or 0.' });
    min(path.maxPrice, 0, { message: 'Maximum price cannot be negative or 0.' });
    min(path.minimuQuantityPerUser, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.minAvailableQuantity, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.maxAvailableQuantity, 0, { message: 'Maximum available quantity cannot be negative.' });
    min(path.minReservedQuantity, 0, { message: 'Minimum reserved quantity cannot be negative.' });
    min(path.maxReservedQuantity, 0, { message: 'Maximum reserved quantity cannot be negative.' });
  });

  private buildFilters() {
    if (this.queryVendor() != null) {
      this.adminVariantFilter.update(filter => ({
        ...filter,
        VendorId: this.queryVendor()
      }));
    }
    this.adminVariantFilter.update(filter => ({
      ...filter,
      approvalStatusId: this.status() == null && this.deleted() == false ? this.draftstatus() : this.status(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: filter.searchTerm ? filter.searchTerm.trim().toLowerCase() : filter.searchTerm,
    }));
  }

  clearFilterValues(): void {
    this.productStatusId.set(null);
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);
    this.hasIssues.set(null);
    this.draftstatus.set(null);
    this.isAvailableForSale.set(null);
    this.isReturn.set(null);
    this.isExchange.set(null);
    this.subCategories.set([]);
    this.adminVariantFilter.set(new AdminProductVariantFilter());
    this.adminVariantFilter.update(model => ({
      ...model,
      statusId: null,
      hasIssues: null,
      isAvailableForSale: null,
      categoryId: null,
      subCategoryId: null
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
    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);

    this.adminVariantFilter.update(model => ({
      ...model,
      categoryId: id,
      subCategoryId: null
    }));

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
    this.productSubCategoryId.set(v ? Number(v) : null);
    this.adminVariantFilter.update(model => ({
      ...model,
      subCategoryId: v ? Number(v) : null
    }));
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productApprovalStatusId.set(v ? Number(v) : null);
    this.draftstatus.set(v ? Number(v) : null);
  }

  onProductStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productStatusId.set(v ? Number(v) : null);
    this.adminVariantFilter.update(model => ({
      ...model,
      statusId: v ? Number(v) : null
    }));
  }

  onHasIssuesChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.hasIssues.set(checked);
    this.adminVariantFilter.update(model => ({
      ...model,
      hasIssues: checked
    }));
  }

  onAvailableForSaleChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isAvailableForSale.set(checked ? true : null);
    this.adminVariantFilter.update(model => ({
      ...model,
      isAvailableForSale: checked
    }));
  }

  onIsReturnChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isReturn.set(checked ? true : null);
    this.adminVariantFilter.update(model => ({
      ...model,
      isReturn: checked ? true : null
    }));
  }

  onIsExchangeChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isExchange.set(checked ? true : null);
    this.adminVariantFilter.update(model => ({
      ...model,
      isExchange: checked ? true : null
    }));
  }


  onAddedByVendorUserIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.adminVariantFilter.update(model => ({ ...model, addedByVendorUserId: v ? Number(v) : null }));
  }

  onMainAttributeIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.adminVariantFilter.update(model => ({ ...model, mainProductSubCategoryAttributeId: v ? Number(v) : null }));
  }


  showDeletePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteProductVariantModel = signal(new AdminDeleteVariantModel());
  progress = signal(false);

  deleteForm = form(this.deleteProductVariantModel, (path) => {
    required(path.remark, { message: 'Enter The Remarks' });
  });

  openDeletePopup(productVariantId: number) {
    this.deleteProductVariantModel.update(model => ({
      ...model,
      productVariantId: productVariantId,
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
      productVariantId: this.deleteProductVariantModel().productVariantId,
      remark: this.deleteProductVariantModel().remark
    };

    this.adminProductService.deleteProductVariant(request).subscribe({
      next: () => {
        this.successMessage.set('Product variant deleted successfully. Closing in 3 seconds...');
        setTimeout(() => {
          this.onCancelDelete();
          this.successMessage.set(null);
          this.loadProductVariant();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(', ');
          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.error?.message ?? 'Something went wrong. Please try again.');
        }
        this.progress.set(false);
      }
    });
  }

  onCancelDelete() {
    this.showDeletePopup.set(false);
    this.deleteForm().reset();
    this.deleteProductVariantModel.set(new AdminDeleteVariantModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }


  showReviewPopup = signal(false);
  reviewProductVariantModel = signal(new ReviewProductVariantModel());
  approvalStatusOption = [
    { id: 4, label: 'Accepted' },
    { id: 5, label: 'Rejected' },
  ];

  reviewForm = form(this.reviewProductVariantModel, (path) => {
    required(path.approvalStatusId, { message: 'Enter The Approval Status' });
    required(path.remarks, { message: 'Enter The Remarks' });
    maxLength(path.remarks, 150, { message: 'Maximum 100 characters' });
  });

  openReviewPopup(productVariantId: number) {
    this.reviewProductVariantModel.update(model => ({
      ...model,
      productVariantId: productVariantId,
      approvalStatusId: null,
      remark: ''
    }));
    this.showReviewPopup.set(true);
  }

  aiReview = signal<AiValidationResult | null>(null);
  loadingAi = signal(false);

  runAiReview(productId: number | null) {
    if (!productId) return;

    this.loadingAi.set(true);
    this.aiReview.set(null);
    this.errorMessage.set('');

    this.adminProductService.getAiVariantReview(productId).subscribe({
      next: (res) => {
        this.aiReview.set(res);
        this.loadingAi.set(false);
      },
      error: () => {
        this.errorMessage.set('AI check failed. Please try again.');
        this.loadingAi.set(false);
      }
    });
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
    this.errorMessage.set(errors.join(', '));
    if (this.reviewForm().invalid()) {
      return;
    }
    this.progress.set(true);

    const request = {
      productVariantId: this.reviewProductVariantModel().productVariantId,
      approvalStatusId: Number(this.reviewProductVariantModel().approvalStatusId),
      remarks: this.reviewProductVariantModel().remarks
    };
    this.adminProductService.reviewProductVariant(request).subscribe({
      next: () => {
        this.successMessage.set('Product variant reviewed successfully');
        setTimeout(() => {
          this.onCancelReview();
          this.successMessage.set(null);
          this.loadProductVariant();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(', ');
          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.error?.message ?? 'Something went wrong. Please try again.');
        }
        this.progress.set(false);
      }
    });
  }

  onCancelReview() {
    this.showReviewPopup.set(false);
    this.reviewForm().reset();
    this.reviewProductVariantModel.set(new ReviewProductVariantModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  // ---------------- Table actions ----------------

  actions: TableAction<ProductVariantModel>[] = [
    { label: 'View', color: 'green', action: 'view' },
    { label: 'Delete', color: 'red', action: 'delete', visible: variant => variant.productApprovalStatus !== 'Deleted_By_Admin' && this.status() == null },
    { label: 'Review', color: 'gray', action: 'review', visible: variant => variant.productApprovalStatus === 'Vendor_Approved' && this.status() == 2 }
  ];

  columns: Column[] = [
    { key: 'productVariantId', header: 'ID' },
    { key: 'sku', header: 'SKU' },
    { key: 'productName', header: 'Name' },
    { key: 'productCategoryName', header: 'Category' },
    { key: 'productSubCategoryName', header: 'Sub Category' },
    { key: 'price', header: 'Price' },
    { key: 'availableQuantity', header: 'Stock' },
    { key: 'productVariantApprovalStatus', header: 'Approval' },
    { key: 'productVariantStatus', header: 'Status' },
  ];

  mobileColumns = [...this.columns];

  handleAction(event: { type: string; row: ProductVariantModel }) {
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
    }
  }

  viewProduct(productVariantId: number) {
    this.route.navigate(['/admin/product-variant-details', productVariantId]);
  }
}
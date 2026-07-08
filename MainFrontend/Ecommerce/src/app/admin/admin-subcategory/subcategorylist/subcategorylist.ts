import { Component, signal, computed, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormField, form, required, min, pattern, maxLength } from '@angular/forms/signals';

import { PagedResponse } from '../../../models/paged-response.model';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { AddProductSubCategoryModel } from '../../../models/admin/admin-product-category/add-models/add-subcategory.model';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { AdminProductSubCategoryFilter } from '../../../models/admin/admin-product-category/filter-models/admin-subcategory.filter';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductService } from '../../../services/admin-product.Service';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';

@Component({
  selector: 'app-subcategorylist',
  imports: [FormField, ReactiveFormsModule, FormsModule, PaginationComponent, MobileCardComponent, FilterComponent, DataTableComponent, PopupComponent, HeaderComponent],
  providers: [DatePipe],
  templateUrl: './subcategorylist.html',
  styleUrl: './subcategorylist.css',
})
export class Subcategorylist extends BasePage {

  subcategory = signal<PagedResponse<AdminProductSubCategoryModel> | null>(null);

  ProductCategoryId = signal<number | null>(null);
  ProductSubCategoryId = signal<number | null>(null);
  status = signal<boolean | null>(null);

  totalPages = computed(() => this.subcategory()?.totalPages ?? 1);

  addSubCategoryModel = signal(new AddProductSubCategoryModel());
  categories = signal<AdminProductCategoryModel[]>([]);

  addForm = form(this.addSubCategoryModel, (path) => {
    required(path.productSubCategoryName, { message: 'Enter sub category name', });
    min(path.commissionPercentage, 0, { message: 'Commission percentage must be 0 or above', });
    min(path.productCategoryId, 1, { message: 'Select product category', });
  });

  constructor(private datePipe: DatePipe, private router: ActivatedRoute, private route: Router, private adminCategoryService: AdminProductCategoryService, private adminProductService: AdminProductService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  categoryStatus = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.categoryStatus.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadCategories();
      this.loadSubCategory();
    });
  }
  clearFilterValues(): void {
    this.ProductCategoryId.set(null);
    this.subCategoryFilter.set(new AdminProductSubCategoryFilter());
    this.subCategoryFilter.update(m => ({ ...m, productCategoryId: null }));
  }

  loadSubCategory(): void {
    this.buildFilter();
    this.adminCategoryService.getProductSubCategory(this.subCategoryFilter()).subscribe({
      next: (response: PagedResponse<AdminProductSubCategoryModel>) => {
        this.subcategory.set(response);
        console.log(this.subcategory());
      },
      error: (error) => {
        console.error(error);

        if (error.status === 404) {
          this.subcategory.set({
            items: [],
            totalCount: 0,
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
            totalPages: 1,
          });
        }
      },
    });
  }

  protected loadData(): void {
    this.loadSubCategory();
  }

  loadCategories(): void {
    this.adminProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
      },
      error: (err) => console.log(err)
    });
  }

  subCategoryFilter = signal(new AdminProductSubCategoryFilter())

  filterForm = form(this.subCategoryFilter, (path) => {
    min(path.productCategoryId, 1, { message: 'Category ID must be greater than 0.' });
    min(path.addedByAdminId, 1, { message: 'Admin ID must be greater than 0.' });
    min(path.productSubCategoryId, 1, { message: 'Sub Category ID must be greater than 0.' });
    min(path.maximumCommissionPercentage, 0, { message: 'Percentage cannot be negative or 0.' });
    min(path.minimumCommissionPercentage, 0, { message: 'Percentage price cannot be negative or 0.' });
  });

  private buildFilter() {
    this.subCategoryFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      status: this.categoryStatus(),
    }));
  }
  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.status.set(null);
    } else {
      this.status.set(value === 'true');
    }
  }

  onCategoryFilterChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.subCategoryFilter.update(m => ({ ...m, productCategoryId: value ? Number(value) : null }));
    this.ProductCategoryId.set(value ? Number(value) : null);
  }

  showAddPopup = signal(false);

  errorMessage = signal('');
  successMessage = signal('');
  progress = signal(false);

  openAddPopup(): void {
    this.loadCategories();
    this.showAddPopup.set(true);
  }

  closeAddPopup(): void {
    this.showAddPopup.set(false);
    this.addSubCategoryModel.set(new AddProductSubCategoryModel());
    this.addForm().reset();
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  addSubCategory(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
    const errors = [];
    if (this.addForm.productSubCategoryName().invalid()) {
      errors.push(
        this.addForm.productSubCategoryName().errors()[0].message
      );
    }
    if (this.addForm.commissionPercentage().invalid()) {
      errors.push(
        this.addForm.commissionPercentage().errors()[0].message
      );
    }
    if (!this.addSubCategoryModel().productCategoryId) {
      errors.push("Select Category");
    }
    this.errorMessage.set(errors.join(", "));
    if (this.addForm().invalid() || errors.length > 0) {
      return;
    }
    this.progress.set(true);
    this.adminCategoryService.addSubCategory(this.addSubCategoryModel()).subscribe({
      next: (response: any) => {
        this.successMessage.set('Sub Category added successfully');
        setTimeout(() => {
          this.loadSubCategory();
          this.closeAddPopup();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.errorMessage.set(error?.error?.message ?? "Failed to add sub category");
        this.progress.set(false);
      },
    });
  }

  onCategoryChange(event: Event) {
    const value = Number((event.target as HTMLSelectElement).value);
    this.addSubCategoryModel.update(m => ({
      ...m,
      productCategoryId: value
    }));
  }

  selectedAction = signal<'activate' | 'deactivate' | null>(null);

  confirmPopup() {
    switch (this.selectedAction()) {
      case 'activate':
        this.activateCategory();
        break;

      case 'deactivate':
        this.deactivateCategory();
        break;
    }
  }

  activateCategory() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminCategoryService.activateSubCategory(id).subscribe({
      next: (response: any) => {
        this.loadSubCategory();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }

  deactivateCategory() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminCategoryService.deactivateSubCategory(id).subscribe({
      next: (response: any) => {
        this.loadSubCategory();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }

  actions = computed<TableAction<AdminProductCategoryModel>[]>(() => this.categoryStatus() == null ? [] : [
    { label: 'Deactivate', color: 'red', action: 'deactivate', visible: category => category.isActive },
    { label: 'Activate', color: 'green', action: 'activate', visible: category => !category.isActive }
  ]);
  columns: Column[] = [
    { key: 'productSubCategoryId', header: 'ID' },
    { key: 'productSubCategoryName', header: 'Sub Category' },
    { key: 'productCategoryId', header: 'Category ID' },
    { key: 'commissionPercentage', header: 'Commission' },
    { key: 'addedByAdminId', header: 'Added Admin Id' },
    { key: 'addedUserName', header: 'Added Admin Name' },
    { key: 'isActive', header: 'Status', formatter: (value: boolean) => value ? 'Active' : 'Inactive' },
    { key: 'createdAt', header: 'Created Date', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') }
  ];
  mobileColumns = [...this.columns];

  handleAction(event: { type: string; row: AdminProductSubCategoryModel }) {
    switch (event.type) {
      case 'activate':
        this.selectedAction.set('activate');
        this.selectedId.set(event.row.productSubCategoryId);

        this.popupTitle.set('Activate Product Sub Category');
        this.popupMessage.set('Are you sure you want to activate this product sub category?');
        this.popupConfirmText.set('Activate');
        this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
        this.titleClass.set('text-green-700');

        this.showPopup.set(true);
        break;

      case 'deactivate':
        this.selectedAction.set('deactivate');
        this.selectedId.set(event.row.productSubCategoryId);

        this.popupTitle.set('Deactivate Product Sub Category');
        this.popupMessage.set('Are you sure you want to deactivate this product sub category?');
        this.popupConfirmText.set('Deactivate');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');

        this.showPopup.set(true);
        break;
    }
  }
}
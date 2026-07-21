import { Component, signal, computed, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductCategoryFilter } from '../../../models/admin/admin-product-category/filter-models/admin-category.filter';
import { DatePipe } from '@angular/common';
import { AddProductCategoryModel } from '../../../models/admin/admin-product-category/add-models/add-category.model';
import { FormField, form, maxLength, min, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { CreateCategoryComponent } from '../create-category-component/create-category-component';

@Component({
  selector: 'app-category-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, FilterComponent, PaginationComponent, MobileCardComponent, DataTableComponent, PopupComponent, HeaderComponent, CreateCategoryComponent],
  providers: [DatePipe],
  templateUrl: './category-list.html',
  styleUrl: './category-list.css',
})
export class CategoryList extends BasePage {

  constructor(private route: Router, private adminCategoryService: AdminProductCategoryService, private datePipe: DatePipe, private router: ActivatedRoute) {
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
    this.loadCategory();
  }

  category = signal<PagedResponse<AdminProductCategoryModel> | null>(null);

  totalPages = computed(() => this.category()?.totalPages ?? 1);

  clearFilterValues(): void {
    this.categoryFilter.set(new AdminProductCategoryFilter());
  }

  categoryStatus = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.categoryStatus.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadCategory();
    });
  }
  tableLoading = signal(false);
  loadCategory() {
    this.buildFilter();
    this.tableLoading.set(true);
    this.adminCategoryService.getProductCategory(this.categoryFilter()).subscribe({
      next: (response: any) => {
        this.category.set(response);
        this.scrollToTop();
        this.tableLoading.set(false);
      },
      error: (error) => {
        console.error(error);
        if (error.status == 404) {
          this.category.set({
            items: [],
            totalCount: 0,
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
            totalPages: 1
          });
          this.tableLoading.set(false);
        }
      }
    })
  }

  categoryFilter = signal(new AdminProductCategoryFilter());

  filterForm = form(this.categoryFilter, (path) => {
    min(path.ProductCategoryId, 1, { message: 'Category ID must be greater than 0.' });
    pattern(path.ProductCategoryName, /^[A-Za-z][A-Za-z\s-]*$/, { message: 'Category name can contain only letters, spaces, and hyphens.' });
    maxLength(path.ProductCategoryName, 100, { message: 'Category name cannot exceed 100 characters.' });
    min(path.AddedByAdminId, 1, { message: 'Admin ID must be greater than 0.' });
  });

  private buildFilter() {
    this.categoryFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      status: this.categoryStatus(),
      ProductCategoryName: filter.ProductCategoryName.trim().toLowerCase(),
    }));
  }

  showAddPopup = signal<boolean>(false);
  openAddPopup() {
    this.showAddPopup.set(true);
  }
  closeAddPopup() {
    this.showAddPopup.set(false);
  }

  categoryAdded() {
    this.showAddPopup.set(false);
    this.loadCategory();
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

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  activateCategory() {
    this.errorMessage.set('');
    this.successMessage.set('');

    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.progress.set(true);
    this.adminCategoryService.activateCategory(id).subscribe({
      next: (response: any) => {
        this.successMessage.set("Product Category activated successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.loadCategory();
          this.closePopup();
          this.successMessage.set(null);
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        console.log(error);
        this.progress.set(false);
      }
    })
  }

  deactivateCategory() {
    this.errorMessage.set('');
    this.successMessage.set('');
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.progress.set(true);
    this.adminCategoryService.deactivateCategory(id).subscribe({
      next: (response: any) => {
        this.successMessage.set("Product Category deactivated successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.loadCategory();
          this.closePopup();
          this.successMessage.set(null);
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        console.log(error);
        this.progress.set(false);
      }
    })
  }

  actions = computed<TableAction<AdminProductCategoryModel>[]>(
    () => this.pageTitle() === 'Product Category List' ? [] : [
      { label: 'Deactivate', color: 'red', action: 'deactivate', visible: category => category.isActive },
      { label: 'Activate', color: 'green', action: 'activate', visible: category => !category.isActive }
    ]);

  columns: Column[] = [
    { key: 'productCategoryId', header: 'ID' },
    { key: 'productCategoryName', header: 'Category' },
    { key: 'addedByAdminId', header: 'Added Admin Id' },
    { key: 'addedUserName', header: 'Added Admin Name' },
    { key: 'isActive', header: 'Status', formatter: (value: boolean) => value ? 'Active' : 'Inactive' },
    { key: 'createdAt', header: 'Created Date', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') }
  ];

  mobileColumns = [...this.columns];

  handleAction(event: { type: string; row: AdminProductCategoryModel }) {
    switch (event.type) {
      case 'activate':
        this.selectedAction.set('activate');
        this.selectedId.set(event.row.productCategoryId);

        this.popupTitle.set('Activate Product Category');
        this.popupMessage.set('Are you sure you want to activate this product category?');
        this.popupConfirmText.set('Activate');
        this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
        this.titleClass.set('text-green-700');
        this.loadingText.set('Activating...');

        this.showPopup.set(true);
        break;

      case 'deactivate':
        this.selectedAction.set('deactivate');
        this.selectedId.set(event.row.productCategoryId);

        this.popupTitle.set('Deactivate Product Category');
        this.popupMessage.set('Are you sure you want to deactivate this product category?');
        this.popupConfirmText.set('Deactivate');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');
        this.loadingText.set('Dectivating...');

        this.showPopup.set(true);
        break;
    }
  }
}

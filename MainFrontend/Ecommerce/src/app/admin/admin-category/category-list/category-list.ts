import { Component, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductCategoryFilter } from '../../../models/admin/admin-product-category/filter-models/admin-category.filter';
import { DatePipe } from '@angular/common';
import { AddProductCategoryModel } from '../../../models/admin/admin-product-category/add-models/add-category.model';
import { FormField, form, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';

@Component({
  selector: 'app-category-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, FilterComponent, PaginationComponent, MobileCardComponent, DataTableComponent],
  providers: [DatePipe],
  templateUrl: './category-list.html',
  styleUrl: './category-list.css',
})
export class CategoryList {

  actions: TableAction<AdminProductCategoryModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    }
  ];
  columns: Column[] = [
    {
      key: 'productCategoryId',
      header: 'ID'
    },
    {
      key: 'productCategoryName',
      header: 'Category'
    },
    {
      key: 'addedByAdminId',
      header: 'Added Admin Id'
    },
    {
      key: 'addedUserName',
      header: 'Added Admin Name'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },
    {
      key: 'createdAt',
      header: 'Created Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    }
  ];

  mobileColumns: Column[] = [
    {
      key: 'productCategoryName',
      header: 'Category'
    },
    {
      key: 'addedByAdminId',
      header: 'Added Admin Id'
    },
    {
      key: 'addedUserName',
      header: 'Admin Name'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },
    {
      key: 'createdAt',
      header: 'Date Created',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    }
  ];

  category = signal<PagedResponse<AdminProductCategoryModel> | null>(null);

  ProductCategoryName = signal<string>('');
  ProductCategoryId = signal<number | null>(null);
  AddedByAdminId = signal<number | null>(null);
  status = signal<boolean | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.category()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  showActivatePopup = signal(false);
  addCategoryModel = signal(new AddProductCategoryModel());

  constructor(private route: Router, private adminCategoryService: AdminProductCategoryService, private datePipe: DatePipe) {

  }
  ngOnInit() {
    this.loadCategory();
  }
  loadCategory() {
    this.adminCategoryService.getProductCategory(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.category.set(response);
        console.log(this.category());
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
        }
      }
    })
  }
  private buildFilter(): AdminProductCategoryFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      ProductCategoryId: this.ProductCategoryId(),
      ProductCategoryName: this.ProductCategoryName(),
      status: this.status(),
      AddedByAdminId: this.AddedByAdminId()
    }
  }
  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }
  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }
  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadCategory();
    this.closeFilterPanel();
  }
  resetFilters(): void {
    this.pageNumber.set(1);
    this.AddedByAdminId.set(null);
    this.ProductCategoryId.set(null);
    this.status.set(null);
    this.ProductCategoryName.set('');
    this.loadCategory();
    this.closeFilterPanel();
  }
  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages()) {
      return;
    }
    this.pageNumber.set(pageNumber);
    this.loadCategory();
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
    this.loadCategory();
  }
  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadCategory();
  }
  onAdminIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.AddedByAdminId.set(v ? Number(v) : null);
  }
  onCategoryIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.ProductCategoryId.set(v ? Number(v) : null);
  }
  onCategoryNameInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.ProductCategoryName.set(v);
  }
  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.status.set(null);
    }
    else {
      this.status.set(value === 'true');
    }
  }
  openPopup(): void {
    this.showActivatePopup.set(true);
  }
  closePopup(): void {
    this.showActivatePopup.set(false);
  }
  addForm = form(this.addCategoryModel, (path) => {
    required(path.productCategoryName, { message: "Enter The Product Category Name" });
  });
  addCategory() {
    if (this.addForm().invalid()) {
      alert("Category Is Invalid");
      return;
    }
    this.adminCategoryService.addCategory(this.addCategoryModel()).subscribe({
      next: (response: any) => {
        alert("Category added successfully");
        console.log(response);
        this.loadCategory();
        this.closePopup();
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
}

import { Component, signal, computed } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { AdminProductCategoryFilter } from '../../../models/admin/admin-product-category/filter-models/admin-category.filter';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';

@Component({
  selector: 'app-inactive-category',
  imports: [FilterComponent, PaginationComponent, MobileCardComponent, DataTableComponent],
  providers: [DatePipe],
  templateUrl: './inactive-category.html',
  styleUrl: './inactive-category.css',
})
export class InactiveCategory {
  actions: TableAction[] = [
    {
      label: 'Activate',
      color: 'green',
      action: 'activate'
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
  handleAction(event: { type: string; row: AdminProductCategoryModel }) {

    switch (event.type) {

      case 'deactivate':
        this.confirmActivate(event.row.productCategoryId);
        break;

    }

  }
  category = signal<PagedResponse<AdminProductCategoryModel> | null>(null);

  ProductCategoryName = signal<string>('');
  ProductCategoryId = signal<number | null>(null);
  AddedByAdminId = signal<number | null>(null);
  status = signal<boolean | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.category()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  showDeactivatePopup = signal(false);
  selectedCategoryId = signal<number | null>(null);

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
    this.status.set(false);
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
  confirmActivate(id: number) {
    this.selectedCategoryId.set(id);
    this.showDeactivatePopup.set(true);
  }
  closePopup() {
    this.showDeactivatePopup.set(false);
    this.selectedCategoryId.set(null);
  }
  activateCategory() {
    const id = this.selectedCategoryId();
    if (id == null) {
      return;
    }
    this.adminCategoryService.activateCategory(id).subscribe({
      next: (response: any) => {
        this.loadCategory();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
}



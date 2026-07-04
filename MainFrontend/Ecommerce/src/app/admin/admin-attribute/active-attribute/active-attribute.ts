import { Component, computed, signal } from '@angular/core';
import { AdminAttributeModel } from '../../../models/admin/admin-product-category/response/admin-attribute.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { AttributeFilter } from '../../../models/admin/admin-product-category/filter-models/attribute.filter';
import { DatePipe } from '@angular/common';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';

@Component({
  selector: 'app-active-attribute',
  imports: [DataTableComponent, MobileCardComponent, PaginationComponent,FilterComponent],
  providers: [DatePipe],
  templateUrl: './active-attribute.html',
  styleUrl: './active-attribute.css',
})
export class ActiveAttribute {
  actions: TableAction[] = [
    {
      label: 'Deactivate',
      color: 'red',
      action: 'deactivate'
    }
  ];
  columns: Column[] = [
    {
      key: 'attributeMasterId',
      header: 'ID'
    },
    {
      key: 'attributeName',
      header: 'Attribute'
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
      key: 'attributeName',
      header: 'Attribute'
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

  handleAction(event: { type: string; row: AdminAttributeModel }) {

    switch (event.type) {

      case 'deactivate':
        this.confirmDeactivate(event.row.attributeMasterId);
        break;

    }

  }
  attribute = signal<PagedResponse<AdminAttributeModel> | null>(null);
  attributeName = signal<string>('');
  status = signal<boolean | null>(null);
  addedByAdminId = signal<number | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.attribute()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);
  showDeactivatePopup = signal(false);
  selectedAttributeId = signal<number | null>(null);

  constructor(private router: Router, private adminCategoryService: AdminProductCategoryService, private datePipe: DatePipe) {

  }
  ngOnInit() {
    this.loadAttribute();
  }
  loadAttribute() {
    this.adminCategoryService.getAttribute(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.attribute.set(response);
        console.log(response);
      },
      error: (error) => {
        console.error(error);
        if (error.status == 404) {
          this.attribute.set({
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
  private buildFilter(): AttributeFilter {
    this.status.set(true);
    return {
      pageNumber:this.pageNumber(),
      pageSize:this.pageSize(),
      attributeName: this.attributeName(),
      status: this.status(),
      addedByAdminId: this.addedByAdminId()
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
    this.loadAttribute();
    this.closeFilterPanel();
  }
  resetFilters(): void {
    this.pageNumber.set(1);
    this.addedByAdminId.set(null);
    this.status.set(null);
    this.attributeName.set('');
    this.loadAttribute();
    this.closeFilterPanel();
  }
  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages()) {
      return;
    }
    this.pageNumber.set(pageNumber);
    this.loadAttribute();
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
    this.loadAttribute();
  }
  onAdminIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.addedByAdminId.set(v ? Number(v) : null);
  }
  onAttributeNameInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.attributeName.set(v);
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
  confirmDeactivate(id: number) {
    this.selectedAttributeId.set(id);
    this.showDeactivatePopup.set(true);
  }
  closePopup() {
    this.showDeactivatePopup.set(false);
    this.selectedAttributeId.set(null);
  }
  deactivateCategory() {
    const id = this.selectedAttributeId();
    if (id == null) {
      return;
    }
    this.adminCategoryService.deactivateAttribute(id).subscribe({
      next: (response: any) => {
        this.loadAttribute();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
}



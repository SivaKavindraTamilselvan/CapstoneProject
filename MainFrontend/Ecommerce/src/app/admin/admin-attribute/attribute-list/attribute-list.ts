import { Component, computed, effect, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { AdminAttributeModel } from '../../../models/admin/admin-product-category/response/admin-attribute.model';
import { AttributeFilter } from '../../../models/admin/admin-product-category/filter-models/attribute.filter';
import { PagedResponse } from '../../../models/paged-response.model';
import { AddAttributeModel } from '../../../models/admin/admin-product-category/add-models/add-attribute.model';
import { DatePipe } from '@angular/common';
import { FormField, form, maxLength, min, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';


@Component({
  selector: 'app-attribute-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, DataTableComponent, FilterComponent, PaginationComponent, MobileCardComponent,PopupComponent],
  templateUrl: './attribute-list.html',
  providers: [DatePipe],
  styleUrl: './attribute-list.css',
})
export class AttributeList extends BasePage {
  constructor(private router: ActivatedRoute, private route: Router, private adminCategoryService: AdminProductCategoryService, private datePipe: DatePipe) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }
  actions = computed<TableAction<AdminAttributeModel>[]>(() => {
    if (this.categoryStatus() == null) {
      return [];
    }

    return [
      {
        label: 'Deactivate',
        color: 'red',
        action: 'deactivate',
        visible: category => category.isActive 
      },
      {
        label: 'Activate',
        color: 'green',
        action: 'activate',
        visible: category => !category.isActive
      }
    ];
  });
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

  attribute = signal<PagedResponse<AdminAttributeModel> | null>(null);
  attributeName = signal<string>('');
  status = signal<boolean | null>(null);
  addedByAdminId = signal<number | null>(null);


  totalPages = computed(() => this.attribute()?.totalPages ?? 1);

  showActivatePopup = signal(false);
  addAttributeModel = signal(new AddAttributeModel());
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
  clearFilterValues(): void {
    this.attributeFilter.set(new AttributeFilter());
  }

  handleAction(event: { type: string; row: AdminAttributeModel }) {
    switch (event.type) {
      case 'activate':
        this.selectedAction.set('activate');
        this.selectedId.set(event.row.attributeMasterId);

        this.popupTitle.set('Activate Attribute');
        this.popupMessage.set('Are you sure you want to activate this attribute?');
        this.popupConfirmText.set('Activate');
        this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
        this.titleClass.set('text-green-700');

        this.showPopup.set(true);
        break;

      case 'deactivate':
        this.selectedAction.set('deactivate');
        this.selectedId.set(event.row.attributeMasterId);

        this.popupTitle.set('Deactivate Attribute');
        this.popupMessage.set('Are you sure you want to deactivate this attribute?');
        this.popupConfirmText.set('Deactivate');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');

        this.showPopup.set(true);
        break;
    }
  }

  protected loadData(): void {
    this.loadAttribute();
  }

  attributeFilter = signal(new AttributeFilter());

  filterForm = form(this.attributeFilter, (path) => {
    pattern(path.attributeName, /^[A-Za-z][A-Za-z\s-]*$/, { message: 'Category name can contain only letters, spaces, and hyphens.' });
    maxLength(path.attributeName, 100, { message: 'Category name cannot exceed 100 characters.' });
    min(path.addedByAdminId, 1, { message: 'Admin ID must be greater than 0.' });
  });

  private buildFilter() {
    this.attributeFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      status: this.categoryStatus(),
      attributeName: filter.attributeName.trim().toLowerCase(),
    }));
  }

  categoryStatus = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.categoryStatus.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadAttribute();
    });
  }

  loadAttribute() {
    this.buildFilter();
    this.adminCategoryService.getAttribute(this.attributeFilter()).subscribe({
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
  openAddPopup(): void {
    this.showActivatePopup.set(true);
  }
  closeAddPopup(): void {
    this.showActivatePopup.set(false);
  }
  addForm = form(this.addAttributeModel, (path) => {
    required(path.attributeName, { message: "Enter The Attribute Name" });
  });
  addAttribute() {
    if (this.addForm().invalid()) {
      alert("Category Is Invalid");
      return;
    }
    this.adminCategoryService.addAttribute(this.addAttributeModel()).subscribe({
      next: (response: any) => {
        alert("Category added successfully");
        console.log(response);
        this.loadAttribute();
        this.closePopup();
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  deactivateCategory() {
    const id = this.selectedId();
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
  activateCategory() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminCategoryService.activateAttribute(id).subscribe({
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

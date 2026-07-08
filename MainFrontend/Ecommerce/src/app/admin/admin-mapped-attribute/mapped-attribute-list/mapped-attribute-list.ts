import { Component, computed, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { AddMapedAttributeModel } from '../../../models/admin/admin-product-category/add-models/add-mapped.model';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { MappedAttributeFilter } from '../../../models/admin/admin-product-category/filter-models/mapped-attribute.filter';
import { form, FormField, maxLength, min, pattern, required } from '@angular/forms/signals';
import { AdminProductService } from '../../../services/admin-product.Service';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { AdminAttributeModel } from '../../../models/admin/admin-product-category/response/admin-attribute.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

export interface GroupedSubCategory {
  subCategoryId: number;
  subCategoryName: string;
  isActive: boolean;
  attributes: AdminMappedAttributeModel[];
}

@Component({
  selector: 'app-mapped-attribute-list',
  imports: [CommonModule, PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent, PopupComponent, FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './mapped-attribute-list.html',
  styleUrl: './mapped-attribute-list.css',
})
export class MappedAttributeList extends BasePage {

  actions = computed<TableAction<AdminProductCategoryModel>[]>(() => {
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
      key: 'productSubCategoryAttributeId',
      header: 'ID'
    },
    {
      key: 'productSubCategoryId',
      header: 'Sub Category Id'
    },
    {
      key: 'productSubCategoryName',
      header: 'Sub Category'
    },
    {
      key: 'attributeMasterId',
      header: 'Attribute Id'
    },
    {
      key: 'attributeName',
      header: 'Attribute Name'
    },
    {
      key: 'addedByAdminId',
      header: 'Added Admin Id'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },

  ];

  mobileColumns: Column[] = [
    {
      key: 'productSubCategoryId',
      header: 'Sub Category Id'
    },
    {
      key: 'productSubCategoryName',
      header: 'Sub Category'
    },
    {
      key: 'attributeMasterId',
      header: 'Attribute Id'
    },
    {
      key: 'attributeName',
      header: 'Attribute Name'
    },
    {
      key: 'addedByAdminId',
      header: 'Added Admin Id'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },
  ];

  masterattribute = signal<PagedResponse<AdminAttributeModel> | null>(null);
  attribute = signal<PagedResponse<AdminMappedAttributeModel> | null>(null);
  status = signal<boolean | null>(null);
  addedByAdminId = signal<number | null>(null);
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  attributeMasterId = signal<number | null>(null);


  totalPages = computed(() => this.attribute()?.totalPages ?? 1);

  showActivatePopup = signal(false);
  addAttributeModel = signal(new AddMapedAttributeModel());

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  attributeMappedFilter = signal(new MappedAttributeFilter());

  clearFilterValues(): void {
    this.attributeMasterId.set(null);
    this.attributeMappedFilter.set(new MappedAttributeFilter());
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);
    this.attributeMappedFilter.update(filter => ({
      ...filter,
      productSubCategoryId: null,
      attributeMasterId:null,
    }));
  }

  categorySelectionRequired = computed(() => {
  return this.productCategoryId() !== null && this.productSubCategoryId() === null;
});


  constructor(private router: ActivatedRoute, private route: Router, private adminCategoryService: AdminProductCategoryService, private adminProductService: AdminProductService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
    effect(() => {
    if (this.categorySelectionRequired()) {
      this.filterErrorMessage.set('Select the Sub category before submitting');
    } else if (this.filterForm().invalid()) {
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
      this.loadAttribute();
      this.loadAttributes();
    });
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

  loadAttribute() {
    this.buildFilter();
    this.adminCategoryService.getmappedAttribute(this.attributeMappedFilter()).subscribe({
      next: (response: any) => {
        this.attribute.set(response);
      },
      error: (error) => {
        console.error(error);
        if (error.status === 404) {
          this.attribute.set({
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

  private buildFilter() {
    this.attributeMappedFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      status: this.categoryStatus(),
    }));
  }

  handleAction(event: { type: string; row: AdminMappedAttributeModel }) {
    switch (event.type) {
      case 'activate':
        this.selectedAction.set('activate');
        this.selectedId.set(event.row.productSubCategoryAttributeId);

        this.popupTitle.set('Activate Mapped Attribute');
        this.popupMessage.set('Are you sure you want to activate this mapped attribute?');
        this.popupConfirmText.set('Activate');
        this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
        this.titleClass.set('text-green-700');

        this.showPopup.set(true);
        break;

      case 'deactivate':
        this.selectedAction.set('deactivate');
        this.selectedId.set(event.row.productSubCategoryAttributeId);

        this.popupTitle.set('Deactivate Mapped Attribute');
        this.popupMessage.set('Are you sure you want to deactivate this mapped attribute?');
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

  filterForm = form(this.attributeMappedFilter, (path) => {
    min(path.attributeMasterId, 1, { message: 'ID must be greater than 0.' });
    min(path.productSubCategoryId, 1, { message: 'ID must be greater than 0.' });
    min(path.addedByAdminId, 1, { message: 'Admin ID must be greater than 0.' });
  });


  onAdminIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.addedByAdminId.set(v ? Number(v) : null);
  }

  onSubCategoryIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
  }

  onAttributeMasterIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.attributeMasterId.set(v ? Number(v) : null);
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.status.set(value === '' ? null : value === 'true');
  }

  openAddPopup(): void {
    this.showActivatePopup.set(true);
  }

  closeAddPopup(): void {
    this.productCategoryId.set(null);
    this.showActivatePopup.set(false);
  }

  addForm = form(this.addAttributeModel, (path) => {
    required(path.attributeMasterId, { message: 'Enter The Attribute Master Id' });
    required(path.productSubCategoryId, { message: 'Enter The Product Sub Category Id' });
  });

  addAttribute() {
    if (this.addForm().invalid()) {
      alert('Form is invalid');
      return;
    }
    this.adminCategoryService.addMappedAttribute(this.addAttributeModel()).subscribe({
      next: (response: any) => {
        alert('Mapped attribute added successfully');
        this.loadAttribute();
        this.closePopup();
      },
      error: (error) => {
        console.error(error);
      },
    });
  }
  loadCategories(): void {

    this.adminProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
        console.log(this.categories);
      },
      error: (err) => console.log(err)
    });
  }

  otherErrorMessage = signal<string | null>(null);
  categoryErrorMessage = signal<string | null>(null);

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);
    this.subCategories.set([]);
    this.attributeMappedFilter.update(filter => ({
      ...filter,
      productCategoryId: id,
      productSubCategoryId: null
    }));

    if (id) {
      this.adminProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res.items ?? res),
        error: (err) => console.log(err)
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    if (this.otherErrorMessage() == null) {
      this.filterErrorMessage.set(null);
    }
    else {
      this.filterErrorMessage.set(this.otherErrorMessage());
    }
    const v = (event.target as HTMLSelectElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
    this.attributeMappedFilter.update(filter => ({
      ...filter,
      productSubCategoryId: v ? Number(v) : null
    }));
  }
  onAddFormSubcategoryChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.addAttributeModel.update(m => ({
      ...m,
      productSubCategoryId: value
    }));
  }
  onFilterFormAttributeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.attributeMappedFilter.update(m => ({
      ...m,
      attributeMasterId: value
    }));
    this.attributeMasterId.set(value);
  }
  onAddFormAttributeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.addAttributeModel.update(m => ({
      ...m,
      attributeMasterId: value
    }));
  }
  loadAttributes() {
    this.adminProductService.getAttribute().subscribe({
      next: (response: any) => {
        this.masterattribute.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }

  activateCategory() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminCategoryService.activateMappedAttribute(id).subscribe({
      next: (response: any) => {
        this.loadAttribute();
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
    this.adminCategoryService.deactivateMappedAttribute(id).subscribe({
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
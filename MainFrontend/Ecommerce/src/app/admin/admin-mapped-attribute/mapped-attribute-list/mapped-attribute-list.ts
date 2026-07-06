import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { AddMapedAttributeModel } from '../../../models/admin/admin-product-category/add-models/add-mapped.model';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { Router } from '@angular/router';
import { MappedAttributeFilter } from '../../../models/admin/admin-product-category/filter-models/mapped-attribute.filter';
import { form, FormField, required } from '@angular/forms/signals';
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

export interface GroupedSubCategory {
  subCategoryId: number;
  subCategoryName: string;
  isActive: boolean;
  attributes: AdminMappedAttributeModel[];
}

@Component({
  selector: 'app-mapped-attribute-list',
  imports: [CommonModule,PaginationComponent,FilterComponent,DataTableComponent,MobileCardComponent],
  templateUrl: './mapped-attribute-list.html',
  styleUrl: './mapped-attribute-list.css',
})
export class MappedAttributeList {

  actions: TableAction<AdminMappedAttributeModel>[] = [
        {
          label: 'View',
          color: 'blue',
          action: 'view'
        },
        {
          label: 'Activate',
          color: 'green',
          action: 'activate'
        }
      ];
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
      handleAction(event: { type: string; row: AdminMappedAttributeModel }) {
    
        
      }

  masterattribute = signal<PagedResponse<AdminAttributeModel> | null>(null);
  attribute = signal<PagedResponse<AdminMappedAttributeModel> | null>(null);
  status = signal<boolean | null>(null);
  addedByAdminId = signal<number | null>(null);
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  attributeMasterId = signal<number | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.attribute()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  showActivatePopup = signal(false);
  addAttributeModel = signal(new AddMapedAttributeModel());

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  constructor(private router: Router, private adminCategoryService: AdminProductCategoryService, private adminProductService: AdminProductService) {

  }

  ngOnInit() {
    this.loadAttribute();
    this.loadCategories();
    this.loadAttributes();
  }

  loadAttribute() {
    this.adminCategoryService.getmappedAttribute(this.buildFilter()).subscribe({
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

  private buildFilter(): MappedAttributeFilter {
    return {
      productSubCategoryId: this.productSubCategoryId(),
      attributeMasterId: this.attributeMasterId(),
      status: this.status(),
      addedByAdminId: this.addedByAdminId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
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
    this.attributeMasterId.set(null);
    this.productSubCategoryId.set(null);
    this.loadAttribute();
    this.closeFilterPanel();
  }

  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages()) return;
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

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadAttribute();
  }

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

  openPopup(): void {
    this.showActivatePopup.set(true);
  }

  closePopup(): void {
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

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);         // reset subcategory when category changes
    this.subCategories.set([]);
    if (id) {
      this.adminProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res.items ?? res),
        error: (err) => console.log(err)
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
  }
  onAddFormSubcategoryChange(event: Event): void {
   const value = Number((event.target as HTMLSelectElement).value);
    this.addAttributeModel.update(m => ({
      ...m,
      productSubCategoryId:value
    }));
  }
   onAddFormAttributeChange(event: Event): void {
   const value = Number((event.target as HTMLSelectElement).value);
    this.addAttributeModel.update(m => ({
      ...m,
      attributeMasterId:value
    }));
  }
  loadAttributes(){
    this.adminProductService.getAttribute().subscribe({
     next:(response:any)=>{
      this.masterattribute.set(response);
     },
     error : (error)=>{
      console.error(error);
     }
    })
  }
}
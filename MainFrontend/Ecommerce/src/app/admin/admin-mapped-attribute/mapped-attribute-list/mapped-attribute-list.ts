import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminMappedAttributeModel } from '../../../models/admin-mapped.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { AddMapedAttributeModel } from '../../../models/add-mapped.model';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { Router } from '@angular/router';
import { MappedAttributeFilter } from '../../../models/mapped-attribute.filter';
import { form, FormField, required } from '@angular/forms/signals';

export interface GroupedSubCategory {
  subCategoryId: number;
  subCategoryName: string;
  isActive: boolean;
  attributes: AdminMappedAttributeModel[];
}

@Component({
  selector: 'app-mapped-attribute-list',
  imports: [CommonModule,FormField],
  templateUrl: './mapped-attribute-list.html',
  styleUrl: './mapped-attribute-list.css',
})
export class MappedAttributeList {
  attribute = signal<PagedResponse<AdminMappedAttributeModel> | null>(null);
  status = signal<boolean | null>(null);
  addedByAdminId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  attributeMasterId = signal<number | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.attribute()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  showActivatePopup = signal(false);
  addAttributeModel = signal(new AddMapedAttributeModel());

  /** Groups flat items into subcategory buckets for the card layout */
  groupedItems = computed<GroupedSubCategory[]>(() => {
    const items = this.attribute()?.items ?? [];
    const map = new Map<number, GroupedSubCategory>();

    for (const item of items) {
      if (!map.has(item.productSubCategoryId)) {
        map.set(item.productSubCategoryId, {
          subCategoryId: item.productSubCategoryId,
          subCategoryName: item.productSubCategoryName,
          isActive: item.isSubCategoryActive,
          attributes: [],
        });
      }
      map.get(item.productSubCategoryId)!.attributes.push(item);
    }

    return Array.from(map.values());
  });

  constructor(
    private router: Router,
    private adminCategoryService: AdminProductCategoryService
  ) {}

  ngOnInit() {
    this.loadAttribute();
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
}
import { Component, signal,computed } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/admin-subcategory.model';
import { Router } from '@angular/router';
import { AdminProductCategoryService } from '../../../services/admin-category.Service';
import { AdminProductSubCategoryFilter } from '../../../models/admin/admin-product-category/admin-subcategory.filter';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-active-sub-category',
  imports: [DatePipe],
  templateUrl: './active-sub-category.html',
  styleUrl: './active-sub-category.css',
})
export class ActiveSubCategory {
  subcategory = signal<PagedResponse<AdminProductSubCategoryModel> | null>(null);

  ProductCategoryId = signal<number | null>(null);
  ProductSubCategoryId = signal<number | null>(null);
  AddedByAdminId = signal<number | null>(null);
  status = signal<boolean | null>(null);
  minimumCommissionPercentage = signal<number | null>(null);
  maximumCommissionPercentage = signal<number | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.subcategory()?.totalPages ?? 1);

  filterPanelOpen = signal<boolean>(false);

  constructor(private route: Router, private adminCategoryService: AdminProductCategoryService) {
    
  }

  ngOnInit(): void {
    this.loadSubCategory();
  }

  loadSubCategory(): void {
    this.adminCategoryService.getProductSubCategory(this.buildFilter()).subscribe({
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

  private buildFilter(): AdminProductSubCategoryFilter {
    this.status.set(true);
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      status: this.status(),
      addedByAdminId: this.AddedByAdminId(),
      productCategoryId: this.ProductCategoryId(),
      productSubCategoryId: this.ProductSubCategoryId(),
      minimumCommissionPercentage: this.minimumCommissionPercentage(),
      maximumCommissionPercentage: this.maximumCommissionPercentage(),
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
    this.loadSubCategory();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.pageNumber.set(1);
    this.AddedByAdminId.set(null);
    this.ProductCategoryId.set(null);
    this.ProductSubCategoryId.set(null);
    this.status.set(null);
    this.minimumCommissionPercentage.set(null);
    this.maximumCommissionPercentage.set(null);

    this.loadSubCategory();
    this.closeFilterPanel();
  }

  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages()) {
      return;
    }

    this.pageNumber.set(pageNumber);
    this.loadSubCategory();
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
    this.loadSubCategory();
  }

  onAdminIdInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.AddedByAdminId.set(value ? Number(value) : null);
  }

  onCategoryIdInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.ProductCategoryId.set(value ? Number(value) : null);
  }

  onSubCategoryIdInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.ProductSubCategoryId.set(value ? Number(value) : null);
  }

  onMinimumCommissionInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.minimumCommissionPercentage.set(value ? Number(value) : null);
  }

  onMaximumCommissionInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.maximumCommissionPercentage.set(value ? Number(value) : null);
  }
}

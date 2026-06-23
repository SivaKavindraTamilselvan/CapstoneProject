import { Component, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorProductVariantModel } from '../../../models/vendor-variant.model';
import { Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductVariantFilter } from '../../../models/vendor/vendor-product/vendor.varaint.filter';
import { DatePipe, DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-variant-list',
  imports: [DatePipe,DecimalPipe],
  templateUrl: './variant-list.html',
  styleUrl: './variant-list.css',
})
export class VariantList {
  variant = signal<PagedResponse<VendorProductVariantModel> | null>(null);
  sku = signal<string>('');
  productId = signal<number | null>(null);
  searchTearm = signal<string>('');
  categoryId = signal<number | null>(null);
  subcategoryId = signal<number | null>(null);
  statusId = signal<number | null>(null);
  approvalstatusId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  minimuQuantityPerUser = signal<number | null>(null);
  addedByVendorUserId = signal<number | null>(null);
  minReservedQuantity = signal<number | null>(null);
  minAvailableQuantity = signal<number | null>(null);
  maxAvailableQuantity = signal<number | null>(null);
  maxReservedQuantity = signal<number | null>(null);
  mainProductSubCategoryAttributeId = signal<number | null>(null);
  isAvailableForSale = signal<boolean | null>(null);
  hasIssues = signal<boolean | null>(null);
  isReturn = signal<boolean | null>(null);
  isExchange = signal<boolean | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.variant()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  constructor(private route: Router, private vendorProductService: VendorProductService) { }

  ngOnInit(): void {
    this.loadProductVariant();
  }

  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' },
    { id: 6, label: 'Deleted By Admin' },
  ];

  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  loadProductVariant(): void {
    this.vendorProductService.getProductVariant(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.variant.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
  private buildFilter(): VendorProductVariantFilter {
    return {
      sku: this.sku(),
      productId: this.productId(),
      searchTerm: this.searchTearm() || null,
      categoryId: this.categoryId(),
      subCategoryId: this.subcategoryId(),
      statusId: this.statusId(),
      approvalStatusId: this.approvalstatusId(),
      addedByVendorUserId: this.addedByVendorUserId(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
      hasIssues: this.hasIssues(),
      minimuQuantityPerUser: this.minimuQuantityPerUser(),
      isAvailableForSale: this.isAvailableForSale(),
      minAvailableQuantity: this.minAvailableQuantity(),
      maxAvailableQuantity: this.maxAvailableQuantity(),
      minReservedQuantity: this.minReservedQuantity(),
      maxReservedQuantity: this.maxReservedQuantity(),
      mainProductSubCategoryAttributeId: this.mainProductSubCategoryAttributeId(),
      isExchange: this.isExchange(),
      isReturn: this.isReturn(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }
  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadProductVariant();
    this.closeFilterPanel();
  }
  resetFilters(): void {
    this.sku.set('');
    this.productId.set(null);
    this.searchTearm.set('');
    this.categoryId.set(null);
    this.subcategoryId.set(null);
    this.statusId.set(null);
    this.approvalstatusId.set(null);
    this.addedByVendorUserId.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.hasIssues.set(null);
    this.minimuQuantityPerUser.set(null);
    this.isAvailableForSale.set(null);
    this.minAvailableQuantity.set(null);
    this.maxAvailableQuantity.set(null);
    this.minReservedQuantity.set(null);
    this.maxReservedQuantity.set(null);
    this.mainProductSubCategoryAttributeId.set(null);
    this.isExchange.set(true);
    this.isReturn.set(true);
    this.pageNumber.set(1);
    this.loadProductVariant();
    this.closeFilterPanel();
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadProductVariant();
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
    this.loadProductVariant();
  }
   onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minPrice.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxPrice.set(v ? Number(v) : null);
  }

  onHasIssuesChange(event: Event): void {
    this.hasIssues.set((event.target as HTMLInputElement).checked || null);
  }

  onAvailableForSaleChange(event: Event): void {
    this.isAvailableForSale.set((event.target as HTMLInputElement).checked || null);
  }
  onSearchTermInput(event: Event): void {
    this.searchTearm.set((event.target as HTMLInputElement).value);
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.approvalstatusId.set(v ? Number(v) : null);
  }
}

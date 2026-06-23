import { Component, OnInit, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { ProductModel } from '../../../models/product.model';
import { Router } from '@angular/router';
import { AdminProductService } from '../../../services/admin-product.Service';
import { AdminProductFilter } from '../../../models/admin/admin-product-category/admin-product.filter'; 

@Component({
  selector: 'app-admin-product',
  imports: [],
  templateUrl: './admin-product.html',
  styleUrl: './admin-product.css',
})
export class AdminProduct implements OnInit {
  products = signal<PagedResponse<ProductModel> | null>(null);

  searchTerm = signal<string>('');
  vendorId = signal<number | null>(null);
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productApprovalStatusId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  filterPanelOpen = signal<boolean>(false);

  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  totalPages = computed(() => this.products()?.totalPages ?? 1);

  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' },
    { id: 6, label: 'Deleted By Admin' },
  ];

  constructor(private route: Router, private adminProductService: AdminProductService) {}

  ngOnInit(): void {
    this.loadProduct();
  }

  private buildFilter(): AdminProductFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm() || null,
      vendorId: this.vendorId(),
      productCategoryId: this.productCategoryId(),
      productSubCategoryId: this.productSubCategoryId(),
      productApprovalStatusId: this.productApprovalStatusId(),
      productStatusId: this.productStatusId(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
      hasIssues: this.hasIssues(),
      isAvailableForSale: this.isAvailableForSale(),
    };
  }

  loadProduct(): void {
    this.adminProductService.getProducts(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.products.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.searchTerm.set('');
    this.vendorId.set(null);
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);
    this.productApprovalStatusId.set(null);
    this.productStatusId.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.hasIssues.set(null);
    this.isAvailableForSale.set(null);
    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadProduct();
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
    this.loadProduct();
  }

  onSearchTermInput(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  onVendorIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.vendorId.set(v ? Number(v) : null);
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productApprovalStatusId.set(v ? Number(v) : null);
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
}
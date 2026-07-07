import { Component, computed, signal } from '@angular/core';
import { UserProductCategoryModel } from '../../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../../models/user/product-category/user-sub-category.model';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { Router } from '@angular/router';
import { UserProductService } from '../../../services/user-product.Service';
import { AdminProductVariantFilter } from '../../../models/admin/admin-product/filter/admin-variant.filter';
import { AdminProductService } from '../../../services/admin-product.Service';
import { DecimalPipe } from '@angular/common';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { ProductVariantModel } from '../../../models/product/product-variant.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { form, FormField, min } from '@angular/forms/signals';

@Component({
  selector: 'app-variant-list',
  imports: [ DataTableComponent, FilterComponent, MobileCardComponent, PaginationComponent, FormField],
  templateUrl: './variant-list.html',
  styleUrl: './variant-list.css',
})
export class VariantList extends BasePage {
  actions: TableAction<ProductVariantModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    },
    {
      label: 'Delete',
      color: 'red',
      action: 'delete'
    }
  ];
  columns: Column[] = [
    {
      key: 'productVariantId',
      header: 'ID'
    },
    {
      key: 'sku',
      header: 'SKU'
    },
    {
      key: 'productName',
      header: 'Name'
    },
    {
      key: 'productCategoryName',
      header: 'Category'
    },
    {
      key: 'productSubCategoryName',
      header: 'Sub Category'
    },
    {
      key: 'price',
      header: 'Price'
    },
    {
      key: 'availableQuantity',
      header: 'Stock'
    },
    {
      key: 'productVariantApprovalStatus',
      header: 'Approval'
    },
    {
      key: 'productVariantStatus',
      header: 'Status'
    },
  ];

  mobileColumns: Column[] = [
    {
      key: 'sku',
      header: 'SKU'
    },
    {
      key: 'productName',
      header: 'Name'
    },
    {
      key: 'productCategoryName',
      header: 'Category'
    },
    {
      key: 'productSubCategoryName',
      header: 'Sub Category'
    },
    {
      key: 'price',
      header: 'Price'
    },
    {
      key: 'availableQuantity',
      header: 'Stock'
    },
    {
      key: 'productVariantApprovalStatus',
      header: 'Approval'
    },
    {
      key: 'productVariantStatus',
      header: 'Status'
    },
  ];
  handleAction(event: { type: string; row: ProductVariantModel }) {
    switch (event.type) {
      case 'view':
        this.viewProduct(event.row.productVariantId);
        break;

    }
  }
  productCategoryId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  categories = signal<UserProductCategoryModel[]>([]);
  subCategories = signal<UserSubProductCategoryModel[]>([]);
  variant = signal<PagedResponse<VendorProductVariantModel> | null>(null);
  sku = signal<string>('');
  productId = signal<number | null>(null);
  vendorId = signal<number | null>(null);
  searchTearm = signal<string>('');
  categoryId = signal<number | null>(null);
  subcategoryId = signal<number | null>(null);
  statusId = signal<number | null>(null);
  approvalstatusId = signal<number | null>(null);

  productApprovalStatusId = signal<number | null>(null);
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

  totalPages = computed(() => this.variant()?.totalPages ?? 1);


  errorMessage = signal<string | null>(null);


  constructor(private route: Router, private adminProductService: AdminProductService, private userProductService: UserProductService) {
    super();
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadProductVariant();
  }

  protected loadData(): void {
    this.loadProductVariant();
  }

  adminVariantFilter = signal(new AdminProductVariantFilter());
  clearFilterValues(): void {
    this.adminVariantFilter.set(new AdminProductVariantFilter());
  }


  loadProductVariant(): void {
    this.adminProductService.getProductVariant(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.variant.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
  private buildFilter(): AdminProductVariantFilter {
    return {
      VendorId: this.vendorId(),
      sku: this.sku(),
      productId: this.productId(),
      searchTerm: this.searchTearm(),
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

  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' },
    { id: 6, label: 'Deleted By Admin' },
  ];

  productStatusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily_Not_Available' },
    { id: 4, label: 'Archived' },
  ];

  filterForm = form(this.adminVariantFilter, (path) => {
    min(path.VendorId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.categoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.subCategoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.statusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.approvalStatusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.minPrice, 0, { message: 'Minimum price cannot be negative or 0.' });
    min(path.maxPrice, 0, { message: 'Maximum price cannot be negative or 0.' });
    min(path.minimuQuantityPerUser, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.minAvailableQuantity, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.maxAvailableQuantity, 0, { message: 'Maximum available quantity cannot be negative.' });
    min(path.minReservedQuantity, 0, { message: 'Minimum reserved quantity cannot be negative.' });
    min(path.maxReservedQuantity, 0, { message: 'Maximum reserved quantity cannot be negative.' });
  });


  onSKUInput(event: Event): void {
    this.sku.set((event.target as HTMLInputElement).value);
  }

  onSearchTermInput(event: Event): void {
    this.searchTearm.set((event.target as HTMLInputElement).value);
  }

  onProductIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.productId.set(v ? Number(v) : null);
  }

  onCategoryIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.categoryId.set(v ? Number(v) : null);
  }

  onSubcategoryIdInput(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.subcategoryId.set(v ? Number(v) : null);
  }

  onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minPrice.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxPrice.set(v ? Number(v) : null);
  }

  onMinAvailableQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minAvailableQuantity.set(v ? Number(v) : null);
  }

  onMaxAvailableQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxAvailableQuantity.set(v ? Number(v) : null);
  }

  onMinReservedQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minReservedQuantity.set(v ? Number(v) : null);
  }

  onMaxReservedQuantityInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxReservedQuantity.set(v ? Number(v) : null);
  }

  onMinimumQuantityPerUserInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minimuQuantityPerUser.set(v ? Number(v) : null);
  }

  onAddedByVendorUserIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.addedByVendorUserId.set(v ? Number(v) : null);
  }

  onMainAttributeIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.mainProductSubCategoryAttributeId.set(v ? Number(v) : null);
  }

  onHasIssuesChange(event: Event): void {
    this.hasIssues.set((event.target as HTMLInputElement).checked || null);
  }

  onAvailableForSaleChange(event: Event): void {
    this.isAvailableForSale.set((event.target as HTMLInputElement).checked || null);
  }

  onStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.statusId.set(v ? Number(v) : null);
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.approvalstatusId.set(v ? Number(v) : null);
  }

  onIsReturnChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isReturn.set(checked ? true : null);
  }

  onIsExchangeChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.isExchange.set(checked ? true : null);
  }

  loadCategories(): void {
    this.userProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res);
        console.log(this.categories);
      },
      error: (error) => {
        if (error.status === 0) {
          this.errorMessage.set(
            'Unable to load categories. Check your internet connection.'
          );
        }
        else {
          this.errorMessage.set(
            'Failed to load product categories.'
          );
        }
      }
    });
  }

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.categoryId.set(id);
    this.subcategoryId.set(null);
    this.subCategories.set([]);
    if (id) {
      this.userProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res),
        error: (error) => {
          if (error.status === 0) {
            this.errorMessage.set(
              'Unable to load subcategories. Check your internet connection.'
            );
          }
          else {
            this.errorMessage.set(
              'Failed to load product subcategories.'
            );
          }
        }
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.subcategoryId.set(v ? Number(v) : null);
  }
  viewProduct(productId: number) {
    this.route.navigate(['/admin/product-details', productId]);
  }
}


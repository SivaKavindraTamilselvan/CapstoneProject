import { Component, computed, signal } from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { DatePipe } from '@angular/common';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorUserModel } from '../../../models/admin/vendor/vendor-user.model';
import { AdminVendorUserFilter } from '../../../models/admin/vendor/vendor-user.filter';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { DetailedCardComponenet } from '../../../shared-components/detailed-card-componenet/detailed-card-componenet';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';

@Component({
  selector: 'app-vendor-details',
  imports: [DetailedCardComponenet],
  providers: [DatePipe],
  templateUrl: './vendor-details.html',
  styleUrl: './vendor-details.css',
})
export class VendorDetails {
  vendorId = signal<number | null>(null);
  vendor = signal(new AdminVendorModel());
  vendorUsers = signal<PagedResponse<AdminVendorUserModel> | null>(null);

  filterPanelOpen = signal<boolean>(false);
  filterapplied = signal<boolean>(false);
  vendorRoleId = signal<number | null>(null);
  isActive = signal<boolean | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(5);
  totalPages = computed(() => this.vendorUsers()?.totalPages ?? 1);
  errorMessage = signal<string | null>(null);

  vendorRoleOption = [
    { id: 1, label: 'Owner' },
    { id: 2, label: 'Manager' },
    { id: 3, label: 'Product Manager' },
    { id: 4, label: 'Order Manager' },
    { id: 5, label: 'Return  Manager' },
    { id: 6, label: 'Refund Manager' },
    { id: 7, label: 'Inventory Manager' },
    { id: 8, label: 'Coupon Manager' },
  ]
  constructor(private datePipe: DatePipe, private adminVendorService: AdminVendorService, private route: ActivatedRoute, private router: Router) {

  }
  ngOnInit() {
    const vendorId = Number(this.route.snapshot.paramMap.get('id'));
    if (vendorId) {
      this.vendorId.set(vendorId);
      this.loadVendor();
      this.loadVendorUser();
    }
  }
  loadVendor() {
    const vendorid = this.vendorId();
    if (vendorid == null) {
      return;
    }
    this.adminVendorService.getVendorDetails(vendorid).subscribe({
      next: (response: any) => {
        this.vendor.set(response);
        console.log(response);
      },
      error: (error) => {
        if (error.status == 404) {
          this.errorMessage.set("Admin User Is Not Found");
        }
      }
    })
  }
  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages()) {
      return;
    }

    this.pageNumber.set(pageNumber);
    this.loadVendorUser();
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
    this.loadVendor();
  }
  loadVendorUser() {
    const vendorid = this.vendorId();
    if (vendorid == null) {
      return;
    }
    this.adminVendorService.getVendorUser(this.buildFilter()).subscribe({
      next: (response: any) => {
        console.log(response);
        this.vendorUsers.set(response);
      }
    })
  }
  private buildFilter(): AdminVendorUserFilter {
    return {
      vendorId: this.vendorId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: this.isActive(),
      vendorRoleId: this.vendorRoleId()
    };
  }
  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  applyFilter(): void {
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadVendorUser();
    this.closeFilterPanel();
  }

  resetFilter(): void {
    this.filterapplied.set(false);
    this.pageNumber.set(1);
    this.isActive.set(null);
    this.vendorRoleId.set(null);
    this.pageSize.set(5);
    this.loadVendorUser();
    this.closeFilterPanel();
  }
  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (value === '') {
      this.isActive.set(null);
    } else {
      this.isActive.set(value === 'true');
    }
  }

  onvendorRoleChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.vendorRoleId.set(value ? Number(value) : null);
  }

  columns: Column[] = [
    { key: 'vendorId', header: 'ID' },
    { key: 'vendorCompanyName', header: 'Company Name' },
    { key: 'contactPersonName', header: 'Contact Person' },
    { key: 'companyEmail', header: 'Email' },
    { key: 'companyPhoneNumber', header: 'Phone Number' },
    { key: 'approvalStatusName', header: 'Approval' },
    { key: 'isActive', header: 'Status', formatter: value => value ? 'Active' : 'Inactive' },
    { key: 'createdAt', header: 'Created Date', formatter: value => this.datePipe.transform(value, 'dd MMM yyyy, hh:mm a') ?? '' },
    { key: 'reviewedByAdminId', header: 'Reviewed Admin Id', },
    { key: 'reviewAdminName', header: 'Reviewed Admin Name', },
  ];

  viewProducts(vendorId: number) {
    this.router.navigate(['/admin/products/list'],
      {
        queryParams: 
        {
          vendorId: vendorId
        }
      }
    );
  }
  viewProductVariant(vendorId: number) {
    this.router.navigate(['/admin/product-variant/list'],
      {
        queryParams: 
        {
          vendorId: vendorId
        }
      }
    );
  }
  viewOrder(vendorId: number) {
    this.router.navigate(['/admin/orders/list'],
      {
        queryParams: 
        {
          vendorId: vendorId
        }
      }
    );
  }
  goBack(): void {
    this.router.navigate(['/admin/vendors/list']);
  }
}

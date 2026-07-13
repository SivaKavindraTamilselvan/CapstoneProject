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
import { ReviewVendorModel } from '../../../models/admin/vendor/review.vendor.model';
import { AdminDeleteVendorModel } from '../../../models/admin/vendor/delete-vendor.model';
import { form, maxLength, required } from '@angular/forms/signals';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';
import { DeletePopupComponents } from '../../../shared-components/delete-popup-components/delete-popup-components';

@Component({
  selector: 'app-vendor-details',
  imports: [DetailedCardComponenet,ReviewPopupComponent,DeletePopupComponents],
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
    window.scrollTo(0, 0);
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
        //console.log(response);
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
        //console.log(response);
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

  approvalStatusOption = [
    { id: 2, label: 'Accepted' },
    { id: 3, label: 'Rejected' },
  ];

  showDeletePopup = signal(false);
  showReviewPopup = signal(false);

  successMessage = signal<string>('');
  //errorMessage = signal<string>('');
  progress = signal(false);

  deleteVendorModel = signal(new AdminDeleteVendorModel());

  deleteForm = form(this.deleteVendorModel, (path) => {
    required(path.remark, { message: 'Enter The Remarks' });
    maxLength(path.remark, 150, { message: 'Maximum 100 characters' });
  });

  openDeletePopup(vendorId: number) {
    this.deleteVendorModel.update(model => ({
      ...model,
      vendorId,
      remark: '',
    }));
    this.showDeletePopup.set(true);
  }

  onConfirmDelete() {
    this.errorMessage.set('');
    this.successMessage.set('');

    const errors = [];
    if (this.deleteForm.remark().invalid()) {
      errors.push(this.deleteForm.remark().errors()[0].message);
    }
    this.errorMessage.set(errors.join(', '));

    if (this.deleteForm().invalid()) {
      return;
    }
    this.progress.set(true);
    const request = {
      vendorId: this.deleteVendorModel().vendorId,
      remark: this.deleteVendorModel().remark,
    };
    this.adminVendorService.DeleteVendor(request).subscribe({
      next: () => {
        this.successMessage.set('Vendor deleted successfully');
        setTimeout(() => {
          this.onCancelDelete();
          this.successMessage.set('');
          this.loadVendor();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set('');

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(', ');
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error.error?.message ?? 'Something went wrong. Please try again.');
        }

        this.progress.set(false);
      },
    });
  }
  onCancelDelete() {
    this.showDeletePopup.set(false);
    this.deleteForm().reset();
    this.deleteVendorModel.set(new AdminDeleteVendorModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  reviewVendorModel = signal(new ReviewVendorModel());

  reviewForm = form(this.reviewVendorModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    maxLength(path.remark, 150, { message: "Maximum 100 characters" });
  })

  openReviewPopup(vendorId: number) {
    this.reviewVendorModel.update(model => ({
      ...model,
      vendorId: vendorId,
      approvalStatusId: null,
      remark: ''
    }));
    this.showReviewPopup.set(true);
  }

  onConfirmReview() {
    this.errorMessage.set('');
    this.successMessage.set('');
    const errors = [];
    if (this.reviewForm.approvalStatusId().invalid()) {
      errors.push(this.reviewForm.approvalStatusId().errors()[0].message);
    }
    if (this.reviewForm.remark().invalid()) {
      errors.push(this.reviewForm.remark().errors()[0].message);
    }
    this.errorMessage.set(errors.join(", "));
    if (this.reviewForm().invalid()) {
      return;
    }
    this.progress.set(true);
    const request = {
      vendorId: this.reviewVendorModel().vendorId,
      approvalStatusId: Number(this.reviewVendorModel().approvalStatusId),
      remark: this.reviewVendorModel().remark.trim()
    };
    this.adminVendorService.reviewVendor(request).subscribe({
      next: () => {
        this.successMessage.set("Vendor reviewed successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.onCancelReview();
          this.successMessage.set('');
          this.loadVendor();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set('');

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
        this.progress.set(false);
      }
    });
  }

  onCancelReview() {
    this.showReviewPopup.set(false);
    this.reviewForm().reset();
    this.reviewVendorModel.set(new ReviewVendorModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }
}

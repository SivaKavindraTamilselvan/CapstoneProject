import { Component, computed, effect, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { CouponListModel } from '../../../models/admin/admin-coupon/get-coupon.model';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminCouponService } from '../../../services/admin-coupon.Service';
import { AdminCouponFilter } from '../../../models/admin/admin-coupon/coupon.filter';
import { DatePipe } from '@angular/common';
import { UpdateCouponModel } from '../../../models/admin/admin-coupon/update-coupon.model';
import { form, FormField, maxLength, min, pattern } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { UpdateCouponComponent } from '../update-coupon-component/update-coupon-component';

@Component({
  selector: 'app-coupon-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, FilterComponent, PaginationComponent, DataTableComponent, MobileCardComponent, PopupComponent, HeaderComponent, UpdateCouponComponent],
  providers: [DatePipe],
  templateUrl: './coupon-list.html',
  styleUrl: './coupon-list.css',
})
export class CouponList extends BasePage {

  constructor(private router: ActivatedRoute, private route: Router, private adminCouponService: AdminCouponService, private datePipe: DatePipe) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
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
      this.loadCoupon();
    });
  }

  coupons = signal<PagedResponse<CouponListModel> | null>(null);
  totalPages = computed(() => this.coupons()?.totalPages ?? 1);
  couponTypeId = signal<number | null>(null);
  isExpired = signal<boolean | null>(null);
  isActive = signal<boolean | null>(null);
  validFrom = signal<string | null>(null);
  validTo = signal<string | null>(null);

  showActivatePopup = signal(false);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  protected loadData(): void {
    this.loadCoupon();
  }

  tableLoading = signal(false);
  loadCoupon() {
    this.buildFilter();
    this.tableLoading.set(true);

    if (this.filterErrorMessage()) {
      return; // don't hit the API with a known-invalid range
    }
    this.progress.set(true);
    this.adminCouponService.getCoupon(this.couponFilter()).subscribe({
      next: (response: PagedResponse<CouponListModel>) => {
        this.coupons.set(response);
        this.progress.set(false);
        this.tableLoading.set(false);
      },
      error: (error) => {
        console.log(error);
        this.progress.set(false);
        if (error.status === 404) {
          this.coupons.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
        else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");
          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.error?.message ?? 'Something went wrong.');
        }
        this.tableLoading.set(false);
      }
    });
  }
  couponFilter = signal(new AdminCouponFilter());
  clearFilterValues(): void {
    this.isExpired.set(null);
    this.isActive.set(null);
    this.couponTypeId.set(null);
    this.couponFilter.set(new AdminCouponFilter());
  }
  couponTypeOption = [
    { id: 1, label: 'Admin' },
    { id: 2, label: 'Vendor' },

  ]

  filterForm = form(this.couponFilter, (path) => {
    min(path.couponId, 1, { message: 'Coupon ID must be greater than 0.' });
    pattern(path.search, /^[A-Za-z0-9\s-]*$/, { message: 'Search can contain only letters, numbers, spaces, and hyphens.' });
    maxLength(path.search, 100, { message: 'Search cannot exceed 100 characters.' });
    min(path.couponTypeId, 1, { message: 'Coupon type ID must be greater than 0.' });
    min(path.minDiscountValue, 0, { message: 'Minimum discount value cannot be negative.' });
    min(path.maxDiscountValue, 0, { message: 'Maximum discount value cannot be negative.' });
    min(path.minOrderAmount, 1, { message: 'Minimum order amount cannot be negative or 0' });
    min(path.maxOrderAmount, 1, { message: 'Maximum order amount cannot be negative or 0.' });
  });

  private buildFilter() {
    this.couponFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: this.categoryStatus(),
    }));
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.isActive.set(null);
    } else {
      this.isActive.set(value === 'true');
    }
    this.couponFilter.update(filter => ({
      ...filter,
      isActive: value === '' ? null : value === 'true'
    }));
  }

  onExpiredChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.isExpired.set(null);
    } else {
      this.isExpired.set(value === 'true');
    }
    this.couponFilter.update(filter => ({
      ...filter,
      isExpired: value === '' ? null : value === 'true'
    }));
  }

  onCouponTypeChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.couponTypeId.set(v ? Number(v) : null);
    this.couponFilter.update(filter => ({
      ...filter,
      couponTypeId: v ? Number(v) : null
    }));
  }

  onValidFromChange(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.validFrom.set(v || null);
    this.couponFilter.update(filter => ({ ...filter, validFrom: v || null }));
    this.validateDateRange();
  }

  onValidToChange(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.validTo.set(v || null);
    this.couponFilter.update(filter => ({ ...filter, validTo: v || null }));
    this.validateDateRange();
  }
  private readonly MIN_VALID_DATE = '2026-06-01';

  private validateDateRange(): void {
    const from = this.couponFilter().validFrom;
    const to = this.couponFilter().validTo;
    const minDate = new Date(this.MIN_VALID_DATE);
    minDate.setHours(0, 0, 0, 0);

    const fromDate = from ? this.stripTime(new Date(from)) : null;
    const toDate = to ? this.stripTime(new Date(to)) : null;

    if (fromDate && fromDate < minDate) {
      this.filterErrorMessage.set('From date cannot be before 01/06/2026.');
      return;
    }

    if (toDate && toDate < minDate) {
      this.filterErrorMessage.set('To date cannot be before 01/06/2026.');
      return;
    }

    if (fromDate && toDate && fromDate > toDate) {
      this.filterErrorMessage.set('From date cannot be later than To date.');
      return;
    }

    this.filterErrorMessage.set(null);
  }

  private stripTime(date: Date): Date {
    date.setHours(0, 0, 0, 0);
    return date;
  }


  showUpdateCouponPopup = signal(false);
  selectedCouponIdForUpdate = signal<number | null>(null);

  openUpdatePopup(couponId: number) {
    this.selectedCouponIdForUpdate.set(couponId);
    this.showUpdateCouponPopup.set(true);
  }

  closeUpdateCouponPopup() {
    this.showUpdateCouponPopup.set(false);
    this.selectedCouponIdForUpdate.set(null);
  }

  onCouponUpdated() {
    this.loadCoupon();
  }


  successMessage = signal('');

  deactivateCoupon() {
    this.errorMessage.set('');
    this.successMessage.set('');
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.progress.set(true);
    this.adminCouponService.deactivateCoupon(id).subscribe({
      next: (response: any) => {
        this.successMessage.set("Coupon deactivated successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.loadCoupon();
          this.closePopup();
          this.successMessage.set('');
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        console.log(error);
        this.errorMessage.set(error?.error?.message ?? "Failed to deactivate coupon");
        this.progress.set(false);
      }
    })
  }

  activateCoupon() {
    this.errorMessage.set('');
    this.successMessage.set('');
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.progress.set(true);
    this.adminCouponService.activateCoupon(id).subscribe({
      next: (response: any) => {
        this.successMessage.set("Coupon activated successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.loadCoupon();
          this.closePopup();
          this.successMessage.set('');
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        console.log(error);
        this.errorMessage.set(error?.error?.message ?? "Failed to activate coupon");
        this.progress.set(false);
      }
    })
  }
  selectedAction = signal<'activate' | 'deactivate' | null>(null);


  confirmPopup() {
    switch (this.selectedAction()) {
      case 'activate':
        this.activateCoupon();
        break;

      case 'deactivate':
        this.deactivateCoupon();
        break;
    }
  }

  handleAction(event: { type: string; row: CouponListModel }) {
    switch (event.type) {
      case 'activate':
        this.selectedAction.set('activate');
        this.selectedId.set(event.row.couponId);

        this.popupTitle.set('Activate Coupon');
        this.popupMessage.set('Are you sure you want to activate this coupon?');
        this.popupConfirmText.set('Activate');
        this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
        this.titleClass.set('text-green-700');
        this.loadingText.set('Activating...');

        this.showPopup.set(true);
        break;

      case 'deactivate':
        this.selectedAction.set('deactivate');
        this.selectedId.set(event.row.couponId);

        this.popupTitle.set('Deactivate Coupon');
        this.popupMessage.set('Are you sure you want to deactivate this coupon?');
        this.popupConfirmText.set('Deactivate');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');
        this.loadingText.set('Deativating...');

        this.showPopup.set(true);
        break;
      case 'update':
        this.openUpdatePopup(event.row.couponId);
        break;
      case 'view':
        this.viewCoupon(event.row.couponId);
        break;
    }
  }

  actions = computed<TableAction<CouponListModel>[]>(() =>
    this.categoryStatus() == null
      ? [
        { label: 'View', color: 'gray', action: 'view' },
        { label: 'Update', color: 'blue', action: 'update' }
      ]
      : [
        { label: 'View', color: 'gray', action: 'view' },
        { label: 'Deactivate', color: 'red', action: 'deactivate', visible: category => category.isActive },
        { label: 'Activate', color: 'green', action: 'activate', visible: category => !category.isActive }
      ]
  );

  columns: Column[] = [
    { key: 'couponId', header: 'ID' },
    { key: 'couponCode', header: 'Code' },
    { key: 'couponTypeName', header: 'Type' },
    { key: 'discountValue', header: 'Discount' },
    { key: 'minimumOrderAmount', header: 'Min Order Amt' },
    { key: 'startDate', header: 'Start Date', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') },
    { key: 'endDate', header: 'End Date', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') },
    { key: 'usageCount', header: 'Usage Count' },
    { key: 'isActive', header: 'Status', formatter: (value: boolean) => (value ? 'Active' : 'Inactive') }
  ];
  mobileColumns = [...this.columns];

  viewCoupon(couponId: number) {
    this.route.navigate(['/admin/coupons', couponId]);
  }

}
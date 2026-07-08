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

@Component({
  selector: 'app-coupon-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, FilterComponent, PaginationComponent, DataTableComponent, MobileCardComponent, PopupComponent],
  providers: [DatePipe],
  templateUrl: './coupon-list.html',
  styleUrl: './coupon-list.css',
})
export class CouponList extends BasePage {
  actions = computed<TableAction<CouponListModel>[]>(() => {
    if (this.categoryStatus() == null) {
      return [
        {
        label: 'View',
        color: 'green',
        action: 'view'
      },

        {
          label: 'Update',
          color: 'blue',
          action: 'update'
        }
      ];
    }

    return [
      {
        label: 'View',
        color: 'green',
        action: 'view'
      },
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
      key: 'couponId',
      header: 'ID'
    },
    {
      key: 'couponCode',
      header: 'Code'
    },
    {
      key: 'couponTypeName',
      header: 'Type'
    },
    {
      key: 'discountValue',
      header: 'Discount'
    },
    {
      key: 'minimumOrderAmount',
      header: 'Min Order Amt'
    },
    {
      key: 'startDate',
      header: 'Start Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    },
    {
      key: 'endDate',
      header: 'End Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    },
    {
      key: 'usageCount',
      header: 'Usage Count'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },

  ];

  mobileColumns: Column[] = [
    {
      key: 'couponCode',
      header: 'Code'
    },
    {
      key: 'couponTypeName',
      header: 'Type'
    },
    {
      key: 'discountValue',
      header: 'Discount'
    },
    {
      key: 'minimumOrderAmount',
      header: 'Min Order Amt'
    },
    {
      key: 'startDate',
      header: 'Start Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    },
    {
      key: 'endDate',
      header: 'End Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    },
    {
      key: 'usageCount',
      header: 'Usage Count'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },
  ];

  coupons = signal<PagedResponse<CouponListModel> | null>(null);


  totalPages = computed(() => this.coupons()?.totalPages ?? 1);


  search = signal<string>("");
  couponTypeId = signal<number | null>(null);
  isExpired = signal<boolean | null>(null);
  isActive = signal<boolean | null>(null);
  couponId = signal<number | null>(null);
  validFrom = signal<Date | null>(null);
  validTo = signal<Date | null>(null);
  minDiscountValue = signal<number | null>(null);
  maxDiscountValue = signal<number | null>(null);
  minOrderAmount = signal<number | null>(null);
  maxOrderAmount = signal<number | null>(null);

  showActivatePopup = signal(false);
  filtererrorMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);

  showUpdateCouponPopup = signal(false);
  updateCouponModel = signal(new UpdateCouponModel());
  successMessage = signal<string | null>(null);
  updateerrorMessage = signal<string | null>(null);
  selectedCouponId = signal<number | null>(null);

  couponFilter = signal(new AdminCouponFilter());
  clearFilterValues(): void {
    this.isExpired.set(null);
    this.isActive.set(null);
    this.couponTypeId.set(null);
    this.couponFilter.set(new AdminCouponFilter());
  }

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

  couponTypeOption = [
    { id: 1, label: 'Admin' },
    { id: 2, label: 'Vendor' },

  ]

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

  protected loadData(): void {
    this.loadCoupon();
  }

  filterForm = form(this.couponFilter, (path) => {
    min(path.couponId, 1, { message: 'Coupon ID must be greater than 0.' });
    pattern(path.search, /^[A-Za-z0-9\s-]*$/, { message: 'Search can contain only letters, numbers, spaces, and hyphens.' });
    maxLength(path.search, 100, { message: 'Search cannot exceed 100 characters.' });
    min(path.couponTypeId, 1, { message: 'Coupon type ID must be greater than 0.' });
    min(path.minDiscountValue, 0, { message: 'Minimum discount value cannot be negative.' });
    min(path.maxDiscountValue, 0, { message: 'Maximum discount value cannot be negative.' });
    min(path.minOrderAmount, 0, { message: 'Minimum order amount cannot be negative.' });
    min(path.maxOrderAmount, 0, { message: 'Maximum order amount cannot be negative.' });
  });

  private buildFilter() {

    this.couponFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: this.categoryStatus(),
    }));
  }

  loadCoupon() {
    this.buildFilter();

    this.progress.set(true);
    this.adminCouponService.getCoupon(this.couponFilter()).subscribe({
      next: (response: PagedResponse<CouponListModel>) => {
        this.coupons.set(response);
        this.progress.set(false);
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
      }
    });
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

  onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.search.set(value);
  }

  onCouponTypeChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.couponTypeId.set(v ? Number(v) : null);
    this.couponFilter.update(filter => ({
      ...filter,
      couponTypeId: v ? Number(v) : null
    }));
  }

  onMinDiscountValueInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minDiscountValue.set(v ? Number(v) : null);
  }

  onMaxDiscountValueInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxDiscountValue.set(v ? Number(v) : null);
  }

  onMinOrderAmountInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minOrderAmount.set(v ? Number(v) : null);
  }

  onMaxOrderAmountInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxOrderAmount.set(v ? Number(v) : null);
  }

  onValidFromChange(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.validFrom.set(v ? new Date(v) : null);
    this.couponFilter.update(filter => ({
      ...filter,
      validFrom: v ? new Date(v) : null
    }));
  }

  onValidToChange(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.validTo.set(v ? new Date(v) : null);
    this.couponFilter.update(filter => ({
      ...filter,
      validTo: v ? new Date(v) : null
    }));
  }

  viewCoupon(couponId: number) {
    this.route.navigate(['/admin/coupons', couponId]);
  }

  updateCouponForm = form(this.updateCouponModel, (path) => {
  });

  openUpdatePopup(couponid: number) {

    this.adminCouponService.getCouponById(couponid).subscribe({
      next: (response: any) => {
        this.updateCouponModel.set(
          new UpdateCouponModel(
            response.couponId,
            response.discountValue,
            response.minimumOrderAmount,
            response.startDate?.split('T')[0],
            response.endDate?.split('T')[0],
            response.minimumNumberOfUsage,
            response.couponDescription
          )
        );
      }
    });
    this.showUpdateCouponPopup.set(true);
  }

  updateCoupon() {
    this.adminCouponService
      .updateCoupon(this.updateCouponModel())
      .subscribe({
        next: (response) => {
          console.log('Coupon updated successfully', response);
          this.closeUpdateCouponPopup();
          this.loadCoupon();
        },
        error: (err) => {
          console.error('Failed to update coupon', err);
        }
      });
  }
  closeUpdateCouponPopup() {
    this.showUpdateCouponPopup.set(false);
  }

  deactivateCoupon() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminCouponService.deactivateCoupon(id).subscribe({
      next: (response: any) => {
        this.loadCoupon();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  activateCoupon() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminCouponService.activateCoupon(id).subscribe({
      next: (response: any) => {
        this.loadCoupon();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
}
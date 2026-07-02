import { Component, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { CouponListModel } from '../../../models/admin/admin-coupon/get-coupon.model';
import { Router } from '@angular/router';
import { AdminCouponService } from '../../../services/admin-coupon.Service';
import { AdminCouponFilter } from '../../../models/admin/admin-coupon/coupon.filter';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-deactive-coupon',
  imports: [DatePipe],
  templateUrl: './deactive-coupon.html',
  styleUrl: './deactive-coupon.css',
})
export class DeactiveCoupon {
  coupons = signal<PagedResponse<CouponListModel> | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.coupons()?.totalPages ?? 1);

  filterPanelOpen = signal<boolean>(false);

  search = signal<string>("");
  couponTypeId = signal<number | null>(null);
  isExpired = signal<boolean | null>(null);
  couponId = signal<number | null>(null);
  validFrom = signal<Date | null>(null);
  validTo = signal<Date | null>(null);
  minDiscountValue = signal<number | null>(null);
  maxDiscountValue = signal<number | null>(null);
  minOrderAmount = signal<number | null>(null);
  maxOrderAmount = signal<number | null>(null);

  filtererrorMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);

  showactivatePopup = signal(false);
  selectedCouponId = signal<number | null>(null);

  constructor(private route: Router, private adminCouponService: AdminCouponService) {}

  ngOnInit(): void {
    this.loadCoupon();
  }

  couponTypeOption = [
    { id: 1, label: 'Admin' },
    { id: 2, label: 'Vendor' },
   
  ]

  loadCoupon() {
    this.progress.set(true);
    this.adminCouponService.getCoupon(this.buildFilter()).subscribe({
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

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadCoupon();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.pageNumber.set(page);
    this.loadCoupon();
  }

  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }

  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }

  toggleFilterPanel(): void {
    const wasOpen = this.filterPanelOpen();
    this.filterPanelOpen.update((open) => !open);
    if (wasOpen && !this.filterapplied()) {
      this.resetFilter();
    }
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  applyFilter(): void {
    if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadCoupon();
    this.closeFilterPanel();
  }

  resetFilter(): void {
    this.filtererrorMessage.set(null);
    this.filterapplied.set(false);
    this.search.set("");
    this.isExpired.set(null);
    this.couponId.set(null);
    this.couponTypeId.set(null);
    this.maxDiscountValue.set(null);
    this.maxOrderAmount.set(null);
    this.minDiscountValue.set(null);
    this.minOrderAmount.set(null);
    this.validFrom.set(null);
    this.validTo.set(null);
    this.pageNumber.set(1);
    this.loadCoupon();
  }

  private buildFilter(): AdminCouponFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: false,
      isExpired: this.isExpired(),
      couponId: this.couponId(),
      couponTypeId: this.couponTypeId(),
      search: this.search(),
      minDiscountValue: this.minDiscountValue(),
      maxDiscountValue: this.maxDiscountValue(),
      minOrderAmount: this.minOrderAmount(),
      maxOrderAmount: this.maxOrderAmount(),
      validFrom: this.validFrom(),
      validTo: this.validTo(),
    };
  }

  onExpiredChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.isExpired.set(null);
    } else {
      this.isExpired.set(value === 'true');
    }
  }

  onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.search.set(value);
  }

  onCouponTypeChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.couponTypeId.set(v ? Number(v) : null);
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
  }

  onValidToChange(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.validTo.set(v ? new Date(v) : null);
  }

  viewCoupon(couponId: number) {
    this.route.navigate(['/admin/coupons', couponId]);
  }

  confirmactivate(id: number) {
    this.selectedCouponId.set(id);
    this.showactivatePopup.set(true);
  }
  closePopup() {
    this.showactivatePopup.set(false);
    this.selectedCouponId.set(null);
  }
  activateCoupon() {
    const id = this.selectedCouponId();
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

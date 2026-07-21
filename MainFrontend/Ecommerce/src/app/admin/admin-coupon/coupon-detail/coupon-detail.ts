import { Component, signal } from '@angular/core';
import { CouponDetailModel, CouponListModel } from '../../../models/admin/admin-coupon/get-coupon.model';
import { AdminCouponService } from '../../../services/admin-coupon.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PopupBase } from '../../../shared-class/popup-base-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { UpdateCouponComponent } from '../update-coupon-component/update-coupon-component';

@Component({
  selector: 'app-coupon-detail',
  imports: [DatePipe, PopupComponent, UpdateCouponComponent],
  templateUrl: './coupon-detail.html',
  styleUrl: './coupon-detail.css',
})
export class CouponDetail {
  coupon = signal(new CouponDetailModel());

  constructor(private adminCouponService: AdminCouponService, private route: ActivatedRoute,private router : Router) {}

  ngOnInit() {
    window.scroll(0,0);
    const couponId = Number(this.route.snapshot.paramMap.get('id'));
    if (couponId) {
      this.loadCoupon(couponId);
    }
  }

  tableLoading = signal(false);
  loadCoupon(couponId: number) {
    this.tableLoading.set(true);
    this.adminCouponService.getCouponById(couponId).subscribe({
      next: (response: any) => {
        this.tableLoading.set(false);
        this.coupon.set(response);
      }
    });
  }

  showPopup = signal(false);
  selectedId = signal<number | null>(null);
  selectedAction = signal<'activate' | 'deactivate' | null>(null);

  popupTitle = signal('');
  popupMessage = signal('');
  popupConfirmText = signal('');
  popupButtonClass = signal('');
  titleClass = signal('');
  loadingText = signal('');

  progress = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  openStatusPopup(id: number, action: 'activate' | 'deactivate') {
    this.selectedId.set(id);
    this.selectedAction.set(action);

    if (action === 'activate') {
      this.popupTitle.set('Activate Coupon');
      this.popupMessage.set('Are you sure you want to activate this coupon?');
      this.popupConfirmText.set('Activate');
      this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
      this.titleClass.set('text-green-700');
      this.loadingText.set('Activating...');
    } else {
      this.popupTitle.set('Deactivate Coupon');
      this.popupMessage.set('Are you sure you want to deactivate this coupon?');
      this.popupConfirmText.set('Deactivate');
      this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
      this.titleClass.set('text-red-700');
      this.loadingText.set('Deactivating...');
    }

    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.showPopup.set(true);
  }

  confirmPopup() {
    const id = this.selectedId();
    const action = this.selectedAction();
    if (id == null || action == null) {
      return;
    }

    this.progress.set(true);
    this.errorMessage.set(null);

    const request$ = action === 'activate'
      ? this.adminCouponService.activateCoupon(id)
      : this.adminCouponService.deactivateCoupon(id);

    request$.subscribe({
      next: () => {
        this.successMessage.set(
          action === 'activate'
            ? 'Coupon activated successfully. Closing in 3 seconds...'
            : 'Coupon deactivated successfully. Closing in 3 seconds...'
        );
        this.loadCoupon(this.coupon().couponId);
        this.progress.set(false);

        setTimeout(() => {
          this.successMessage.set(null);
          this.closePopup();
        }, 3000);
      },
      error: (error) => {
        console.log(error);
        this.progress.set(false);
        this.errorMessage.set('Something went wrong. Please try again.');
      }
    });
  }

  closePopup(): void {
    this.showPopup.set(false);
    this.selectedId.set(null);
    this.selectedAction.set(null);

    this.popupTitle.set('');
    this.popupMessage.set('');
    this.popupConfirmText.set('');
    this.popupButtonClass.set('');
    this.titleClass.set('');
    this.loadingText.set('');
    this.errorMessage.set(null);
    this.successMessage.set(null);
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
    this.loadCoupon(this.coupon().couponId);
  }

  goBack() {
    this.router.navigate(['/admin/coupon/list']);
  }

  viewOrder(id : number){
    this.router.navigate(['/admin/order', id]);
  }
}
import { Component, signal } from '@angular/core';
import { CouponDetailModel } from '../../../models/admin/admin-coupon/get-coupon.model';
import { AdminCouponService } from '../../../services/admin-coupon.Service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-coupon-detail',
  imports: [DatePipe],
  templateUrl: './coupon-detail.html',
  styleUrl: './coupon-detail.css',
})
export class CouponDetail {
  coupon = signal(new CouponDetailModel());
  constructor(private adminCouponService: AdminCouponService, private route: ActivatedRoute) {

  }
  ngOnInit(){
    const couponId = Number(this.route.snapshot.paramMap.get('id'));

    if (couponId) {
      this.loadCoupon(couponId);
    }
  }
  loadCoupon(couponId : number){
    this.adminCouponService.getCouponById(couponId).subscribe({
      next : (response:any)=>{
        this.coupon.set(response);
      }
    })
  }
}

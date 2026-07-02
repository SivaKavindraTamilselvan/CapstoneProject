import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CouponDetail } from './coupon-detail';

describe('CouponDetail', () => {
  let component: CouponDetail;
  let fixture: ComponentFixture<CouponDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CouponDetail],
    }).compileComponents();

    fixture = TestBed.createComponent(CouponDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

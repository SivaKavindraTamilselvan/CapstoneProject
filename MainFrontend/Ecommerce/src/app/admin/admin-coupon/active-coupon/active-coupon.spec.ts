import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveCoupon } from './active-coupon';

describe('ActiveCoupon', () => {
  let component: ActiveCoupon;
  let fixture: ComponentFixture<ActiveCoupon>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveCoupon],
    }).compileComponents();

    fixture = TestBed.createComponent(ActiveCoupon);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

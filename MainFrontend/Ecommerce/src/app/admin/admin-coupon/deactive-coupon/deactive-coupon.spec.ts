import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeactiveCoupon } from './deactive-coupon';

describe('DeactiveCoupon', () => {
  let component: DeactiveCoupon;
  let fixture: ComponentFixture<DeactiveCoupon>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeactiveCoupon],
    }).compileComponents();

    fixture = TestBed.createComponent(DeactiveCoupon);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateCouponComponent } from './update-coupon-component';

describe('UpdateCouponComponent', () => {
  let component: UpdateCouponComponent;
  let fixture: ComponentFixture<UpdateCouponComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateCouponComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateCouponComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

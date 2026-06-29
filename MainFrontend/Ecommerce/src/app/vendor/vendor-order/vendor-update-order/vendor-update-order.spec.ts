import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorUpdateOrder } from './vendor-update-order';

describe('VendorUpdateOrder', () => {
  let component: VendorUpdateOrder;
  let fixture: ComponentFixture<VendorUpdateOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorUpdateOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorUpdateOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

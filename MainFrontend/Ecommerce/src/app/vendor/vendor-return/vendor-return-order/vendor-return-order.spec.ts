import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorReturnOrder } from './vendor-return-order';

describe('VendorReturnOrder', () => {
  let component: VendorReturnOrder;
  let fixture: ComponentFixture<VendorReturnOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorReturnOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorReturnOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

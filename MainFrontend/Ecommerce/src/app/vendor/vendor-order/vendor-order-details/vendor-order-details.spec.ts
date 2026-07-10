import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorOrderDetails } from './vendor-order-details';

describe('VendorOrderDetails', () => {
  let component: VendorOrderDetails;
  let fixture: ComponentFixture<VendorOrderDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorOrderDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorOrderDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

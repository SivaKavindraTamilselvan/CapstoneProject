import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorProductDetails } from './vendor-product-details';

describe('VendorProductDetails', () => {
  let component: VendorProductDetails;
  let fixture: ComponentFixture<VendorProductDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorProductDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorProductDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

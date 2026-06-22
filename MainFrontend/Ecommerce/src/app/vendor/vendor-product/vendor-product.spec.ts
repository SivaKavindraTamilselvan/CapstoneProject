import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorProduct } from './vendor-product';

describe('VendorProduct', () => {
  let component: VendorProduct;
  let fixture: ComponentFixture<VendorProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorProduct],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorProduct);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

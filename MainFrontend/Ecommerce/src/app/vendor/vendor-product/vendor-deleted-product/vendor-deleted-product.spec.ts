import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorDeletedProduct } from './vendor-deleted-product';

describe('VendorDeletedProduct', () => {
  let component: VendorDeletedProduct;
  let fixture: ComponentFixture<VendorDeletedProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorDeletedProduct],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorDeletedProduct);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorProductVariant } from './vendor-product-variant';

describe('VendorProductVariant', () => {
  let component: VendorProductVariant;
  let fixture: ComponentFixture<VendorProductVariant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorProductVariant],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorProductVariant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

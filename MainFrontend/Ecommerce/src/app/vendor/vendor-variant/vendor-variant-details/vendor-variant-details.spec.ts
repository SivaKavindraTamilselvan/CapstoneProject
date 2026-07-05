import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorVariantDetails } from './vendor-variant-details';

describe('VendorVariantDetails', () => {
  let component: VendorVariantDetails;
  let fixture: ComponentFixture<VendorVariantDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorVariantDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorVariantDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

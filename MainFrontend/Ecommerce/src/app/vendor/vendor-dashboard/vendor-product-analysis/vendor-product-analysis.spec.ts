import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorProductAnalysis } from './vendor-product-analysis';

describe('VendorProductAnalysis', () => {
  let component: VendorProductAnalysis;
  let fixture: ComponentFixture<VendorProductAnalysis>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorProductAnalysis],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorProductAnalysis);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

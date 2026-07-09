import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorOrderAnalysis } from './vendor-order-analysis';

describe('VendorOrderAnalysis', () => {
  let component: VendorOrderAnalysis;
  let fixture: ComponentFixture<VendorOrderAnalysis>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorOrderAnalysis],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorOrderAnalysis);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

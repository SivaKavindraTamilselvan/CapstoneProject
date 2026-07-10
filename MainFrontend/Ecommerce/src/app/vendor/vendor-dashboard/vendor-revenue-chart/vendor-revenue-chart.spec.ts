import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorRevenueChart } from './vendor-revenue-chart';

describe('VendorRevenueChart', () => {
  let component: VendorRevenueChart;
  let fixture: ComponentFixture<VendorRevenueChart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorRevenueChart],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorRevenueChart);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

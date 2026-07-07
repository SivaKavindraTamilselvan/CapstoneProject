import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminRevenueChart } from './admin-revenue-chart';

describe('AdminRevenueChart', () => {
  let component: AdminRevenueChart;
  let fixture: ComponentFixture<AdminRevenueChart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminRevenueChart],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminRevenueChart);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

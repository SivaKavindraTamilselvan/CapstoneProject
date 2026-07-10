import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersByMonthChartComponent } from './orders-by-month-chart-component';

describe('OrdersByMonthChartComponent', () => {
  let component: OrdersByMonthChartComponent;
  let fixture: ComponentFixture<OrdersByMonthChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrdersByMonthChartComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(OrdersByMonthChartComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

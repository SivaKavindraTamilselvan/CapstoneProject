import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminOrderAnalytics } from './admin-order-analytics';

describe('AdminOrderAnalytics', () => {
  let component: AdminOrderAnalytics;
  let fixture: ComponentFixture<AdminOrderAnalytics>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminOrderAnalytics],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminOrderAnalytics);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

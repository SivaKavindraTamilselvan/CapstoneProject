import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminConfirmedOrders } from './admin-confirmed-orders';

describe('AdminConfirmedOrders', () => {
  let component: AdminConfirmedOrders;
  let fixture: ComponentFixture<AdminConfirmedOrders>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminConfirmedOrders],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminConfirmedOrders);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

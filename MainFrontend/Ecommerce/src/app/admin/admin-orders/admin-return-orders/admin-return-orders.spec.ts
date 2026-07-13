import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminReturnOrders } from './admin-return-orders';

describe('AdminReturnOrders', () => {
  let component: AdminReturnOrders;
  let fixture: ComponentFixture<AdminReturnOrders>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminReturnOrders],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminReturnOrders);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

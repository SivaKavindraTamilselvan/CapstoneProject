import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCancelledOrder } from './admin-cancelled-order';

describe('AdminCancelledOrder', () => {
  let component: AdminCancelledOrder;
  let fixture: ComponentFixture<AdminCancelledOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCancelledOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCancelledOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

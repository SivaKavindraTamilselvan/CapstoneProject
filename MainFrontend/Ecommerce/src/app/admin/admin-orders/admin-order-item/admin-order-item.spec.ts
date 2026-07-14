import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminOrderItem } from './admin-order-item';

describe('AdminOrderItem', () => {
  let component: AdminOrderItem;
  let fixture: ComponentFixture<AdminOrderItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminOrderItem],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminOrderItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

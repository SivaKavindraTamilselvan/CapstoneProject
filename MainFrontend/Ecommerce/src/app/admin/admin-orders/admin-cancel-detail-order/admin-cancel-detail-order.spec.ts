import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCancelDetailOrder } from './admin-cancel-detail-order';

describe('AdminCancelDetailOrder', () => {
  let component: AdminCancelDetailOrder;
  let fixture: ComponentFixture<AdminCancelDetailOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCancelDetailOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCancelDetailOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

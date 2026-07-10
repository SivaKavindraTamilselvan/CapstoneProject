import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminReturnDetailOrder } from './admin-return-detail-order';

describe('AdminReturnDetailOrder', () => {
  let component: AdminReturnDetailOrder;
  let fixture: ComponentFixture<AdminReturnDetailOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminReturnDetailOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminReturnDetailOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

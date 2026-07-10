import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminReturnOrder } from './admin-return-order';

describe('AdminReturnOrder', () => {
  let component: AdminReturnOrder;
  let fixture: ComponentFixture<AdminReturnOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminReturnOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminReturnOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetAdminOrders } from './get-admin-orders';

describe('GetAdminOrders', () => {
  let component: GetAdminOrders;
  let fixture: ComponentFixture<GetAdminOrders>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetAdminOrders],
    }).compileComponents();

    fixture = TestBed.createComponent(GetAdminOrders);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

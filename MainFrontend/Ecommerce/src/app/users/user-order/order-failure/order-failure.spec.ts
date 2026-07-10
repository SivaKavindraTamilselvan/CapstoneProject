import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderFailure } from './order-failure';

describe('OrderFailure', () => {
  let component: OrderFailure;
  let fixture: ComponentFixture<OrderFailure>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderFailure],
    }).compileComponents();

    fixture = TestBed.createComponent(OrderFailure);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

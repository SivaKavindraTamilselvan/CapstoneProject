import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReturnRefundPopup } from './return-refund-popup';

describe('ReturnRefundPopup', () => {
  let component: ReturnRefundPopup;
  let fixture: ComponentFixture<ReturnRefundPopup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReturnRefundPopup],
    }).compileComponents();

    fixture = TestBed.createComponent(ReturnRefundPopup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

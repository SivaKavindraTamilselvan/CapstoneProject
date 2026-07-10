import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdditionalRefundPopup } from './additional-refund-popup';

describe('AdditionalRefundPopup', () => {
  let component: AdditionalRefundPopup;
  let fixture: ComponentFixture<AdditionalRefundPopup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdditionalRefundPopup],
    }).compileComponents();

    fixture = TestBed.createComponent(AdditionalRefundPopup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

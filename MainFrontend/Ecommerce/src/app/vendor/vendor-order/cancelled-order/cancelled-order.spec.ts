import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancelledOrder } from './cancelled-order';

describe('CancelledOrder', () => {
  let component: CancelledOrder;
  let fixture: ComponentFixture<CancelledOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CancelledOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(CancelledOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

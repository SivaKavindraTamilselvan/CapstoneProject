import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorKpiCard } from './vendor-kpi-card';

describe('VendorKpiCard', () => {
  let component: VendorKpiCard;
  let fixture: ComponentFixture<VendorKpiCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorKpiCard],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorKpiCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

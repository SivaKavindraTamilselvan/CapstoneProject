import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminKpiCard } from './admin-kpi-card';

describe('AdminKpiCard', () => {
  let component: AdminKpiCard;
  let fixture: ComponentFixture<AdminKpiCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminKpiCard],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminKpiCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminHomeDashboard } from './admin-home-dashboard';

describe('AdminHomeDashboard', () => {
  let component: AdminHomeDashboard;
  let fixture: ComponentFixture<AdminHomeDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminHomeDashboard],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminHomeDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

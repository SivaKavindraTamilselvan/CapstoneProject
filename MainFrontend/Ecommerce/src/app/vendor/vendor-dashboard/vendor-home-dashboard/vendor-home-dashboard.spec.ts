import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorHomeDashboard } from './vendor-home-dashboard';

describe('VendorHomeDashboard', () => {
  let component: VendorHomeDashboard;
  let fixture: ComponentFixture<VendorHomeDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorHomeDashboard],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorHomeDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

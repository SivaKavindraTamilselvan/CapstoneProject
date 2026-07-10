import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminProductAnalytics } from './admin-product-analytics';

describe('AdminProductAnalytics', () => {
  let component: AdminProductAnalytics;
  let fixture: ComponentFixture<AdminProductAnalytics>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminProductAnalytics],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminProductAnalytics);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

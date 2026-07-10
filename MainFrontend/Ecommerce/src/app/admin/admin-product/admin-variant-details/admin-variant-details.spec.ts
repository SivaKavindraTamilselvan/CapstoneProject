import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminVariantDetails } from './admin-variant-details';

describe('AdminVariantDetails', () => {
  let component: AdminVariantDetails;
  let fixture: ComponentFixture<AdminVariantDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminVariantDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminVariantDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

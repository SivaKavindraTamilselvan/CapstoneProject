import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivateVendorUser } from './activate-vendor-user';

describe('ActivateVendorUser', () => {
  let component: ActivateVendorUser;
  let fixture: ComponentFixture<ActivateVendorUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActivateVendorUser],
    }).compileComponents();

    fixture = TestBed.createComponent(ActivateVendorUser);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

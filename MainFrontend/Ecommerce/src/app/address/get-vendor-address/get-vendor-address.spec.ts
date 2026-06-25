import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetVendorAddress } from './get-vendor-address';

describe('GetVendorAddress', () => {
  let component: GetVendorAddress;
  let fixture: ComponentFixture<GetVendorAddress>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetVendorAddress],
    }).compileComponents();

    fixture = TestBed.createComponent(GetVendorAddress);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorUserDetails } from './vendor-user-details';

describe('VendorUserDetails', () => {
  let component: VendorUserDetails;
  let fixture: ComponentFixture<VendorUserDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorUserDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorUserDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

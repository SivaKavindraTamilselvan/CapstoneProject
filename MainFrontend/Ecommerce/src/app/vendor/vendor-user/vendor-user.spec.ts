import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorUser } from './vendor-user';

describe('VendorUser', () => {
  let component: VendorUser;
  let fixture: ComponentFixture<VendorUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorUser],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorUser);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

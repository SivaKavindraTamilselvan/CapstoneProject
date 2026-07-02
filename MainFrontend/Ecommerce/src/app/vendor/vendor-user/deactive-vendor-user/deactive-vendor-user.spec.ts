import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeactiveVendorUser } from './deactive-vendor-user';

describe('DeactiveVendorUser', () => {
  let component: DeactiveVendorUser;
  let fixture: ComponentFixture<DeactiveVendorUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeactiveVendorUser],
    }).compileComponents();

    fixture = TestBed.createComponent(DeactiveVendorUser);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

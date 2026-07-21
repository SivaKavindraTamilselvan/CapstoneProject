import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorProfilePage } from './vendor-profile-page';

describe('VendorProfilePage', () => {
  let component: VendorProfilePage;
  let fixture: ComponentFixture<VendorProfilePage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorProfilePage],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorProfilePage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

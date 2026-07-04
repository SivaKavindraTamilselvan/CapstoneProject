import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorNotificationList } from './vendor-notification-list';

describe('VendorNotificationList', () => {
  let component: VendorNotificationList;
  let fixture: ComponentFixture<VendorNotificationList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorNotificationList],
    }).compileComponents();

    fixture = TestBed.createComponent(VendorNotificationList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

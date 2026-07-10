import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminNotificationList } from './admin-notification-list';

describe('AdminNotificationList', () => {
  let component: AdminNotificationList;
  let fixture: ComponentFixture<AdminNotificationList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminNotificationList],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminNotificationList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

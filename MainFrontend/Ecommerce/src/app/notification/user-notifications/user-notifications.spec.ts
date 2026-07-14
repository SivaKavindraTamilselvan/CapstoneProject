import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserNotifications } from './user-notifications';

describe('UserNotifications', () => {
  let component: UserNotifications;
  let fixture: ComponentFixture<UserNotifications>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserNotifications],
    }).compileComponents();

    fixture = TestBed.createComponent(UserNotifications);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

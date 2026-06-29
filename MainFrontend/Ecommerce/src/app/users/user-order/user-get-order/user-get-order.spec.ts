import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserGetOrder } from './user-get-order';

describe('UserGetOrder', () => {
  let component: UserGetOrder;
  let fixture: ComponentFixture<UserGetOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserGetOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(UserGetOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

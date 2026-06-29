import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAddOrder } from './user-add-order';

describe('UserAddOrder', () => {
  let component: UserAddOrder;
  let fixture: ComponentFixture<UserAddOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserAddOrder],
    }).compileComponents();

    fixture = TestBed.createComponent(UserAddOrder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

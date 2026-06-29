import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserFavorite } from './user-favorite';

describe('UserFavorite', () => {
  let component: UserFavorite;
  let fixture: ComponentFixture<UserFavorite>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserFavorite],
    }).compileComponents();

    fixture = TestBed.createComponent(UserFavorite);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserProductDetails } from './user-product-details';

describe('UserProductDetails', () => {
  let component: UserProductDetails;
  let fixture: ComponentFixture<UserProductDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserProductDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(UserProductDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

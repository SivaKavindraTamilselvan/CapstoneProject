import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryMobile } from './category-mobile';

describe('CategoryMobile', () => {
  let component: CategoryMobile;
  let fixture: ComponentFixture<CategoryMobile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryMobile],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryMobile);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

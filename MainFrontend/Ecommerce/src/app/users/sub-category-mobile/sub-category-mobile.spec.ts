import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubCategoryMobile } from './sub-category-mobile';

describe('SubCategoryMobile', () => {
  let component: SubCategoryMobile;
  let fixture: ComponentFixture<SubCategoryMobile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubCategoryMobile],
    }).compileComponents();

    fixture = TestBed.createComponent(SubCategoryMobile);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

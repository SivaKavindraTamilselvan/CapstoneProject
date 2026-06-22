import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InactiveSubCategory } from './inactive-sub-category';

describe('InactiveSubCategory', () => {
  let component: InactiveSubCategory;
  let fixture: ComponentFixture<InactiveSubCategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InactiveSubCategory],
    }).compileComponents();

    fixture = TestBed.createComponent(InactiveSubCategory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

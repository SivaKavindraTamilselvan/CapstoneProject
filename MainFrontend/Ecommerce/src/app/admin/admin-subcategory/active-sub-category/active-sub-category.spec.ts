import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveSubCategory } from './active-sub-category';

describe('ActiveSubCategory', () => {
  let component: ActiveSubCategory;
  let fixture: ComponentFixture<ActiveSubCategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveSubCategory],
    }).compileComponents();

    fixture = TestBed.createComponent(ActiveSubCategory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

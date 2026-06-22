import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InactiveCategory } from './inactive-category';

describe('InactiveCategory', () => {
  let component: InactiveCategory;
  let fixture: ComponentFixture<InactiveCategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InactiveCategory],
    }).compileComponents();

    fixture = TestBed.createComponent(InactiveCategory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

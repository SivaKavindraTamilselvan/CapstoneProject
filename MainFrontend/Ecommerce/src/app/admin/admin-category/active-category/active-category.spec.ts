import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveCategory } from './active-category';

describe('ActiveCategory', () => {
  let component: ActiveCategory;
  let fixture: ComponentFixture<ActiveCategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveCategory],
    }).compileComponents();

    fixture = TestBed.createComponent(ActiveCategory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubcategoryBarChartComponent } from './subcategory-bar-chart-component';

describe('SubcategoryBarChartComponent', () => {
  let component: SubcategoryBarChartComponent;
  let fixture: ComponentFixture<SubcategoryBarChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubcategoryBarChartComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SubcategoryBarChartComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

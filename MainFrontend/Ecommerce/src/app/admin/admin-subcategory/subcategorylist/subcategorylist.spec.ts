import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Subcategorylist } from './subcategorylist';

describe('Subcategorylist', () => {
  let component: Subcategorylist;
  let fixture: ComponentFixture<Subcategorylist>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Subcategorylist],
    }).compileComponents();

    fixture = TestBed.createComponent(Subcategorylist);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

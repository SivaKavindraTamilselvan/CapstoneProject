import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddVariant } from './add-variant';

describe('AddVariant', () => {
  let component: AddVariant;
  let fixture: ComponentFixture<AddVariant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddVariant],
    }).compileComponents();

    fixture = TestBed.createComponent(AddVariant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

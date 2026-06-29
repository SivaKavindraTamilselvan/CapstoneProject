import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateVariant } from './update-variant';

describe('UpdateVariant', () => {
  let component: UpdateVariant;
  let fixture: ComponentFixture<UpdateVariant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateVariant],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateVariant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

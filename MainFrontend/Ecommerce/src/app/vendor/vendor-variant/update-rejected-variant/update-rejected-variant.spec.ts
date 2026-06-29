import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateRejectedVariant } from './update-rejected-variant';

describe('UpdateRejectedVariant', () => {
  let component: UpdateRejectedVariant;
  let fixture: ComponentFixture<UpdateRejectedVariant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateRejectedVariant],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateRejectedVariant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

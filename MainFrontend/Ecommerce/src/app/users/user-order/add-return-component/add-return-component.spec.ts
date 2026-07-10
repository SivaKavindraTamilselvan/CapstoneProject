import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddReturnComponent } from './add-return-component';

describe('AddReturnComponent', () => {
  let component: AddReturnComponent;
  let fixture: ComponentFixture<AddReturnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddReturnComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AddReturnComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

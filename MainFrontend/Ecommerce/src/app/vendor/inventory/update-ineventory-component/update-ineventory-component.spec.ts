import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateIneventoryComponent } from './update-ineventory-component';

describe('UpdateIneventoryComponent', () => {
  let component: UpdateIneventoryComponent;
  let fixture: ComponentFixture<UpdateIneventoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateIneventoryComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateIneventoryComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

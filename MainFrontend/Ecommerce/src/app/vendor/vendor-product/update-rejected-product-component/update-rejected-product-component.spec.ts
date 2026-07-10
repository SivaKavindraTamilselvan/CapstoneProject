import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateRejectedProductComponent } from './update-rejected-product-component';

describe('UpdateRejectedProductComponent', () => {
  let component: UpdateRejectedProductComponent;
  let fixture: ComponentFixture<UpdateRejectedProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateRejectedProductComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateRejectedProductComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

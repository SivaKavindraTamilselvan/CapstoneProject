import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateProductComponenet } from './update-product-componenet';

describe('UpdateProductComponenet', () => {
  let component: UpdateProductComponenet;
  let fixture: ComponentFixture<UpdateProductComponenet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateProductComponenet],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateProductComponenet);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

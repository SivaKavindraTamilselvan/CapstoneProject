import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteVendor } from './delete-vendor';

describe('DeleteVendor', () => {
  let component: DeleteVendor;
  let fixture: ComponentFixture<DeleteVendor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteVendor],
    }).compileComponents();

    fixture = TestBed.createComponent(DeleteVendor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

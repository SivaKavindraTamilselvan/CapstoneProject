import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeletePopupComponents } from './delete-popup-components';

describe('DeletePopupComponents', () => {
  let component: DeletePopupComponents;
  let fixture: ComponentFixture<DeletePopupComponents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeletePopupComponents],
    }).compileComponents();

    fixture = TestBed.createComponent(DeletePopupComponents);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

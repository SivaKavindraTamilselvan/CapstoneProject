import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeletedInventory } from './deleted-inventory';

describe('DeletedInventory', () => {
  let component: DeletedInventory;
  let fixture: ComponentFixture<DeletedInventory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeletedInventory],
    }).compileComponents();

    fixture = TestBed.createComponent(DeletedInventory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteAddress } from './delete-address';

describe('DeleteAddress', () => {
  let component: DeleteAddress;
  let fixture: ComponentFixture<DeleteAddress>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteAddress],
    }).compileComponents();

    fixture = TestBed.createComponent(DeleteAddress);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

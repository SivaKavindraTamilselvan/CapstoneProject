import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteProductComponentd } from './delete-product-componentd';

describe('DeleteProductComponentd', () => {
  let component: DeleteProductComponentd;
  let fixture: ComponentFixture<DeleteProductComponentd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteProductComponentd],
    }).compileComponents();

    fixture = TestBed.createComponent(DeleteProductComponentd);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

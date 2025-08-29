import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { ItemDetailsComponent } from './item-details.component';


describe('ItemDetailsComponent', () => {
  let component: ItemDetailsComponent;
  let fixture: ComponentFixture<ItemDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ItemDetailsComponent],
      imports: [TranslateModule.forRoot()]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ItemDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should receive the data for label and value', () => {
    component.label = 'Builder name'
    component.data = "Tom's Builder"
    fixture.detectChanges()
    const paragraphs = fixture.nativeElement.querySelectorAll('p') as HTMLParagraphElement[]
    expect(paragraphs[0].textContent).toBe('Builder name')
    expect(paragraphs[1].textContent).toBe("Tom's Builder")
  })
});

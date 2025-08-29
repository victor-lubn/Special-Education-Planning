import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { IconsModule } from '../../atoms/icons/icons.module';
import { BackWithTitleComponent } from './back-with-title.component';

describe('BackWithTitleComponent', () => {
  let component: BackWithTitleComponent;
  let fixture: ComponentFixture<BackWithTitleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ 
        BackWithTitleComponent,        
      ],
      imports: [
        IconsModule
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BackWithTitleComponent);
    component = fixture.componentInstance;
    component.title = 'Aieps page';
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have the received title', () => {
    expect(component.title).toBe('Aieps page');
  });

  it('should click on the back button', () => {
    spyOn(component, 'onBack');

    const button = fixture.debugElement.query(By.css('.tdp-back-with-title--icon'));
    button.nativeElement.dispatchEvent(new Event('click'));

    expect(component.onBack).toHaveBeenCalled();
  });
});


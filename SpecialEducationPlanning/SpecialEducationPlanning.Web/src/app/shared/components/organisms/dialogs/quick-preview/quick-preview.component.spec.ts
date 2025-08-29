import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { CoreModule } from '../../../../../core/core.module';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { QuickPreviewComponent } from './quick-preview.component'

describe('QuickPreviewComponent', () => {
  let component: QuickPreviewComponent;
  let fixture: ComponentFixture<QuickPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        QuickPreviewComponent, ButtonComponent, ModalComponent, IconComponent
      ],
      imports: [CoreModule, TranslateModule.forRoot({})]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuickPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should change title', () => {
    let title = 'Plan Id'
    component.title = title;
    fixture.detectChanges();
    expect(component.title).toBe(title);
  })

  it('should open print window', () => {
    spyOn(component.onPrint, 'emit')
    const buttonPrint = fixture.nativeElement.querySelectorAll('tdp-button')[0];
    buttonPrint.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onPrint.emit).toHaveBeenCalled();
  });
});

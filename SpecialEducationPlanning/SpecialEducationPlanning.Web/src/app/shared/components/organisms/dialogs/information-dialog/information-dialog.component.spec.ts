import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatTooltip, MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { IconsModule } from '../../../atoms/icons/icons.module';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { InfoDialogComponent } from './information-dialog.component'

describe('InfoDialogComponent', () => {
  let component: InfoDialogComponent;
  let fixture: ComponentFixture<InfoDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        InfoDialogComponent,
        ButtonComponent,
        ModalComponent,
        IconComponent,
        MatTooltip
      ],
      imports: [
        MatTooltipModule,
        TranslateModule.forRoot({}),
        IconsModule
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InfoDialogComponent);
    component = fixture.componentInstance;
    component.accept = 'Accept';
    component.cancel = 'Cancel';
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should close dialog on cancel', () => {
    spyOn(component.onCancel, 'emit')
    const buttonClose = fixture.nativeElement.querySelector('tdp-button');
    buttonClose.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onCancel.emit).toHaveBeenCalled();
  })

});

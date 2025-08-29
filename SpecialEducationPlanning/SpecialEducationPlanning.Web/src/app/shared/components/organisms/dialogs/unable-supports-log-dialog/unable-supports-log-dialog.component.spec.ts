import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { ElectronService } from 'src/app/core/electron-api/electron.service';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { ErrorLogService } from 'src/app/core/services/error-log/error-log.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconsModule } from '../../../atoms/icons/icons.module';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { UnableSupportsLogDialogComponent } from './unable-supports-log-dialog.component';

describe('UnableSupportsLogDialogComponent', () => {
  let component: UnableSupportsLogDialogComponent;
  let fixture: ComponentFixture<UnableSupportsLogDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        ModalComponent,
        ButtonComponent
      ],
      imports: [
        HttpClientTestingModule,
        RouterTestingModule,
        TranslateModule.forRoot(),
        IconsModule,
        MatTooltipModule
      ],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        DialogsService,
        ErrorLogService,
        ElectronService,
        { provide: MatDialog, useValue: {} },
      ]
    })

    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UnableSupportsLogDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

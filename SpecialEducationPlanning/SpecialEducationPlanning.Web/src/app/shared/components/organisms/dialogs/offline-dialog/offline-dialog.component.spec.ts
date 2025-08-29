import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { createTranslateLoader } from 'src/app/app.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { OfflineDialogComponent } from './offline-dialog.component';

describe('OfflineDialogComponent', () => {
    let component: OfflineDialogComponent;
    let fixture: ComponentFixture<OfflineDialogComponent>;

    let data = {
        titleStringKey: '',
        descriptionStringKey: '',
        buttonStringKey: '',
        descriptionStringKey2: ''
    }

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                SharedModule,
                MatDialogModule,
                TranslateModule.forRoot({
                    loader: {
                        provide: TranslateLoader,
                        useFactory: (createTranslateLoader),
                        deps: [HttpClient]
                    }
                }),
                HttpClientModule
            ],
            declarations: [
                OfflineDialogComponent,
                ModalComponent,
                ButtonComponent
            ],
            providers: [
                { provide: MatDialogRef, useValue: {} },
                { provide: MAT_DIALOG_DATA, useValue: data }
            ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(OfflineDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should have default values', () => {
        expect(component.titleKey).toBe('');
        expect(component.descriptionKey).toBe('');
        expect(component.buttonKey).toBe('');
        expect(component.descriptionKey2).toBe('');
    });

    it('should click the action button', () => {
        spyOn(component, 'onActionClick');
        let closeButton = fixture.debugElement.nativeElement.querySelector('.tdp-offline-button');
        closeButton.dispatchEvent(new Event('onClick'));
        fixture.detectChanges();
        expect(component.onActionClick).toHaveBeenCalled();
    });
});

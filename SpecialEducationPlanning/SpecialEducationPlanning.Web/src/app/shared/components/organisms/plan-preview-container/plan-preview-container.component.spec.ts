import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule, } from '@ngx-translate/core';
import { of } from 'rxjs';
import { CommunicationService } from 'src/app/core/services/communication/communication.service';
import { UserInfoService } from 'src/app/core/services/user-info/user-info.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';
import { PlanPreviewContainerComponent } from './plan-preview-container.component';

const fakeUserInfoService: Partial<UserInfoService> = {
    hasPermission:
        jasmine.createSpy('hasPermission').and.returnValue(of(true))
};

describe('PlanPreviewContainerComponent', () => {
    let component: PlanPreviewContainerComponent;
    let fixture: ComponentFixture<PlanPreviewContainerComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                SharedModule,
                TranslateModule.forRoot(),
                HttpClientTestingModule
            ],
            declarations: [
                PlanPreviewContainerComponent, SpinnerComponent, IconComponent, ButtonComponent
            ],
            providers: [
                {
                    provide: UserInfoService,
                    useValue: fakeUserInfoService
                },
                CommunicationService
            ]
        })
        .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(PlanPreviewContainerComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should click edit in Fusion', () => {
        spyOn(component, 'handleEdit');
        let editButton = fixture.debugElement.nativeElement.querySelector('.plan-preview-container-edit-button');
        editButton.dispatchEvent(new Event('onClick'));
        fixture.detectChanges();
        expect(component.handleEdit).toHaveBeenCalled();
    });

    it('should click the action button', () => {
        spyOn(component, 'handlePublish');
        let publishButton = fixture.debugElement.nativeElement.querySelector('.plan-preview-container-publish-button');
        publishButton.dispatchEvent(new Event('onClick'));
        fixture.detectChanges();
        expect(component.handlePublish).toHaveBeenCalled();
    });
});

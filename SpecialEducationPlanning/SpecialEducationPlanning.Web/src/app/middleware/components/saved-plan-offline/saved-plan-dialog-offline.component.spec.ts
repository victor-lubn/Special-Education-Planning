import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { NotificationsService } from "angular2-notifications";
import { of } from "rxjs";
import { UserService } from "../../../core/api/user/user.service";
import { CommunicationService } from "../../../core/services/communication/communication.service";
import { DialogsService } from "../../../core/services/dialogs/dialogs.service";
import { ErrorLogService } from "../../../core/services/error-log/error-log.service";
import { ServiceInjector } from "../../../core/services/service-injector/service-injector";
import { ButtonComponent } from "../../../shared/components/atoms/button/button.component";
import { IconComponent } from "../../../shared/components/atoms/icon/icon.component";
import { InputComponent } from "../../../shared/components/atoms/input/input.component";
import { ModalComponent } from "../../../shared/components/atoms/modal/modal.component";
import { TextAreaComponent } from "../../../shared/components/atoms/text-area/text-area.component";
import { PermissionDirective } from "../../../shared/directives/permission.directive";
import { OfflineMiddlewareCreateActionService } from "../../services/offline-callbacks/create-action-offline.service";
import { OfflineMiddlewareCreatePlanService } from "../../services/offline-callbacks/create-plan-offline.service";
import { OfflineMiddlewareEditPlanService } from "../../services/offline-callbacks/edit-plan-offline.service";
import { OfflineMiddlewareEditVersionNotesService } from "../../services/offline-callbacks/edit-version-notes-offline.service";
import { OfflineMiddlewareGetPlanService } from "../../services/offline-callbacks/get-plan-offline.service";
import { OfflineMiddlewareGetPreviewService } from "../../services/offline-callbacks/get-preview-offline.service";
import { OfflineMiddlewareGetRomService } from "../../services/offline-callbacks/get-rom-offline.service";
import { OfflineMiddlewareReadFileService } from "../../services/offline-callbacks/read-file-offline-message.service";
import { OfflineMiddlewareService } from "../../services/offline-middleware.service";
import { OfflineMiddlewareSavedPlanDialogComponent } from "./saved-plan-dialog-offline.component";

const fakeOfflineMiddlewareService: Partial<OfflineMiddlewareService> = {
  getSinglePlanObservable:
    jasmine.createSpy('getSinglePlanObservable').and.returnValue(of({}))
}

const data = {
  planId: 12,
  planVersionId: 14,
  versionNumber: 1,
  builderName: 'Example Builder',
  catalogType: 'Kitchen',
  isNewPlan: false,
  lineItems: [],
  mainRange: 'test',
  mainUniqueId: '124141',
  romFileInfo: {
    type: 'test',
    fileName: 'testFileName',
    romByteArray: 'file'
  },
  preview: {
    type: 'test',
    fileName: 'testPlanPreview',
    previewByteArray: 'file'
  },
  versionNotes: 'Notes',
  planName: 'Test plan',
  quoteOrderNumber: '1234535532'
}

describe('SavePlanDialogOfflineComponent', () => {
  let component: OfflineMiddlewareSavedPlanDialogComponent;
  let fixture: ComponentFixture<OfflineMiddlewareSavedPlanDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        OfflineMiddlewareSavedPlanDialogComponent, ModalComponent, InputComponent, TextAreaComponent, ButtonComponent, IconComponent, PermissionDirective
      ],
      imports: [
        CommonModule, HttpClientModule, ReactiveFormsModule, FormsModule, RouterTestingModule, MatAutocompleteModule, MatDialogModule,
        TranslateModule.forRoot()
      ],
      providers: [
        CommunicationService,
        { provide: OfflineMiddlewareService, useValue: fakeOfflineMiddlewareService },
        OfflineMiddlewareReadFileService,
        OfflineMiddlewareCreatePlanService,
        OfflineMiddlewareEditPlanService,
        OfflineMiddlewareEditVersionNotesService,
        OfflineMiddlewareGetRomService, 
        OfflineMiddlewareGetPreviewService, 
        OfflineMiddlewareGetPlanService,
        OfflineMiddlewareCreateActionService,
        { provide: NotificationsService, useValue: {} },
        { provide: MatDialog, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: data },
        { provide: MatDialogRef, useValue: {} },
        { provide: DialogsService, useValue: {} },
        { provide: UserService, useValue: {} },
        { provide: ErrorLogService, useValue: {} },
      ]
    });
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OfflineMiddlewareSavedPlanDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have one input', () => {
    const inputElement = fixture.debugElement.nativeElement.querySelectorAll('.tdp-input')
    expect(inputElement.length).toBe(1)
  });

  it('should have one text area', () => {
    const textAreaElement = fixture.debugElement.nativeElement.querySelectorAll('tdp-text-area')
    expect(textAreaElement.length).toBe(1)
  });
  
  it('should discard changes', () => {
    spyOn(component, 'discardVersion');
    const buttonDiscard = fixture.nativeElement.querySelectorAll('tdp-button')[0];
    buttonDiscard.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.discardVersion).toHaveBeenCalled();
  })

  it('should overwrite version', () => {
    spyOn(component, 'overwriteVersion');
    const buttonOverwrite = fixture.nativeElement.querySelectorAll('tdp-button')[1];
    buttonOverwrite.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.overwriteVersion).toHaveBeenCalled();
  })

  it('should save new version', () => {
    spyOn(component, 'saveNewVersion');
    const buttonSaveNew = fixture.nativeElement.querySelectorAll('tdp-button')[2];
    buttonSaveNew.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.saveNewVersion).toHaveBeenCalled();
  })

});

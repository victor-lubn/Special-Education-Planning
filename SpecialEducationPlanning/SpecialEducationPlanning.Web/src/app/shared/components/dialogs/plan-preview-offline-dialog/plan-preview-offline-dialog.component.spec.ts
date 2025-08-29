import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatSpinner } from "@angular/material/progress-spinner";
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { NotificationsService } from "angular2-notifications";
import { of } from "rxjs";
import { ErrorLogService } from "../../../../core/services/error-log/error-log.service";
import { ServiceInjector } from "../../../../core/services/service-injector/service-injector";
import { EducationToolMiddlewareService } from "../../../../middleware/services/Education-tool-middleware.service";
import { OfflineMiddlewareService } from "../../../../middleware/services/offline-middleware.service";
import { ButtonComponent } from "../../atoms/button/button.component";
import { IconComponent } from "../../atoms/icon/icon.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { PlanPreviewOfflineDialogComponent } from "./plan-preview-offline-dialog.component";

const fakeOfflineMiddlewareService: Partial<OfflineMiddlewareService> = {
  getPreviewFileObservable:
    jasmine.createSpy('getPreviewFileObservable').and.returnValue(of({})),
}

const planOffline = {
  id_offline: 12,
  planNumber: '12315151',
  planName: 'Test plan',
  EducationerName: 'Educationer',
  survey: true,
  catalogueId: 13,
  catalogueCode: '121515',
  versions: [],
  lastOpen: null,
  updatedDate: null,
  createdDate: null
}

const versionOffline = {
  id_offline: 12,
  romPath: 'string',
  previewPath: 'string',
  versionNotes: 'Testing notes',
  quoteOrderNumber: '123153532634',
  catalogueCode: '121442141',
  range: 'Balham Gloss',
  romItems: [],
  updatedDate: null,
  createdDate: null
}

const data = { version: versionOffline, plan: planOffline}

describe('PlanPreviewOfflineDialogComponent', () => {
  let component: PlanPreviewOfflineDialogComponent;
  let fixture: ComponentFixture<PlanPreviewOfflineDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        PlanPreviewOfflineDialogComponent, ModalComponent, ButtonComponent, IconComponent, MatSpinner
      ],
      imports: [
        CommonModule, HttpClientModule, ReactiveFormsModule, FormsModule, RouterTestingModule,
        TranslateModule.forRoot()
      ],
      providers: [
        { provide: EducationToolMiddlewareService, useValue: {} },
        { provide: OfflineMiddlewareService, useValue: fakeOfflineMiddlewareService },
        { provide: NotificationsService, useValue: {} },
        { provide: MatDialog, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: data },
        { provide: MatDialogRef, useValue: {} },
        { provide: ErrorLogService, useValue: {} },
      ]
    });
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlanPreviewOfflineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

});


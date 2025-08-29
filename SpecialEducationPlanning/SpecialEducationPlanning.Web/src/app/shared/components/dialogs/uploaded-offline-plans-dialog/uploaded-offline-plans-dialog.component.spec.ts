import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { DialogsService } from "../../../../core/services/dialogs/dialogs.service";
import { ButtonComponent } from "../../atoms/button/button.component";
import { IconComponent } from "../../atoms/icon/icon.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { UploadedOfflinePlansDialogComponent } from "./uploaded-offline-plans-dialog.component";

const syncPlans = [
  { idOffline: 4, planCode: '1231421' },
  { idOffline: 5, planCode: '2414145' }
];

const failedSyncPlans = [ 1, 2, 3];

const data = {syncedPlans: syncPlans, notSyncedPlans: failedSyncPlans}

describe('UploadedOfflinePlansDialogComponent', () => {
  let component: UploadedOfflinePlansDialogComponent;
  let fixture: ComponentFixture<UploadedOfflinePlansDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        UploadedOfflinePlansDialogComponent, ModalComponent, ButtonComponent, IconComponent
      ],
      imports: [
        CommonModule, HttpClientModule, ReactiveFormsModule, FormsModule, RouterTestingModule,
        TranslateModule.forRoot()
      ],
      providers: [
        { provide: MatDialog, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: data },
        { provide: MatDialogRef, useValue: {} },
        { provide: DialogsService, useValue: {} },
      ]
    });
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadedOfflinePlansDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.ngOnInit();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should accept action', () => {
    spyOn(component, 'acceptAction');
    const buttonAccept = fixture.nativeElement.querySelectorAll('tdp-button')[0];
    buttonAccept.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.acceptAction).toHaveBeenCalled();
  })

  it('should go to unassigned plans', () => {
    spyOn(component, 'navigateToUnassignedPlans');
    const buttonGoUnnasigned = fixture.nativeElement.querySelectorAll('tdp-button')[1];
    buttonGoUnnasigned.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.navigateToUnassignedPlans).toHaveBeenCalled();
  })
});

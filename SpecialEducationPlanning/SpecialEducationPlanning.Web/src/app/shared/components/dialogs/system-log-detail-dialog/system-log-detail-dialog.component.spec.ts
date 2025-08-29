import { DatePipe } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { TranslateModule } from "@ngx-translate/core";
import { ApiService } from "../../../../core/api/api.service";
import { UserInfoService } from "../../../../core/services/user-info/user-info.service";
import { TdpDatePipe } from "../../../pipes/pipes";
import { ButtonComponent } from "../../atoms/button/button.component";
import { IconComponent } from "../../atoms/icon/icon.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { SystemLogDetailDialogComponent } from "./system-log-detail-dialog.component"


describe('SystemLogDetailDialogComponent', () => {

  let component: SystemLogDetailDialogComponent;
  let fixture: ComponentFixture<SystemLogDetailDialogComponent>;

  beforeEach(waitForAsync(() => {
    const testBed = TestBed.configureTestingModule({
      imports: [TranslateModule.forRoot(), HttpClientModule],
      declarations: [SystemLogDetailDialogComponent, ModalComponent, IconComponent, ButtonComponent, TdpDatePipe],
      providers: [
        MatDialog,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
        UserInfoService,
        ApiService,
        DatePipe
      ]
    })
    testBed.compileComponents();
    fixture = TestBed.createComponent(SystemLogDetailDialogComponent);
    component = fixture.debugElement.componentInstance;
  }));

  it('should create the component', waitForAsync(() => {
    expect(component).toBeTruthy();
  }));

});
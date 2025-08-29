import { HttpClientModule } from "@angular/common/http";
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { ApiService } from "../../../../core/api/api.service";
import { SharedModule } from "../../../shared.module";
import { ButtonComponent } from "../../atoms/button/button.component";
import { IconsModule } from "../../atoms/icons/icons.module";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { ErrorMessageDialogComponent } from "./error-message-dialog.component";

describe('ErrorMessageDialogComponent', () => {

  let component: ErrorMessageDialogComponent;
  let fixture: ComponentFixture<ErrorMessageDialogComponent>;

  beforeEach(waitForAsync(() => {
      const testBed = TestBed.configureTestingModule({
          imports: [ SharedModule, BrowserAnimationsModule, RouterTestingModule, 
            TranslateModule.forRoot(), MatAutocompleteModule, HttpClientModule,
            IconsModule
          ],
          declarations: [
            ErrorMessageDialogComponent, ButtonComponent, ModalComponent,
          ],
          providers: [
            { provide: ApiService, useValue: {} },
            { provide: MatDialog, useValue: {} },
            { provide: MatDialogRef, useValue: {} },
            { provide: MAT_DIALOG_DATA, useValue: {} },
          ]
      })
      testBed.compileComponents();
      fixture = TestBed.createComponent(ErrorMessageDialogComponent);
      component = fixture.debugElement.componentInstance;
  }));

  it('should create', () => {
      expect(component).toBeTruthy();
    });
});
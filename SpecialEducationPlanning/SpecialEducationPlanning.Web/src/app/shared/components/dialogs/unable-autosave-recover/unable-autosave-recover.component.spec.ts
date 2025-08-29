import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { ButtonComponent } from "../../atoms/button/button.component";
import { IconComponent } from "../../atoms/icon/icon.component";
import { IconsModule } from "../../atoms/icons/icons.module";
import { InputComponent } from "../../atoms/input/input.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { UnableAutosaveRecoverDialogComponent } from "./unable-autosave-recover.component";


describe('UnableAutosaveRecoverDialogComponent', () => {

  let component: UnableAutosaveRecoverDialogComponent;
  let fixture: ComponentFixture<UnableAutosaveRecoverDialogComponent>;

  beforeEach(waitForAsync(() => {
    const testBed = TestBed.configureTestingModule({
      declarations: [UnableAutosaveRecoverDialogComponent, ModalComponent, IconComponent, ButtonComponent, InputComponent],
      imports: [CommonModule, TranslateModule.forRoot(), HttpClientModule, MatAutocompleteModule, RouterTestingModule, IconsModule],
      providers: [
        MatDialog,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} }, 
      ]
    })
    testBed.compileComponents();
    fixture = TestBed.createComponent(UnableAutosaveRecoverDialogComponent);
    component = fixture.debugElement.componentInstance;
  }));

  it('should create the component', waitForAsync(() => {
    expect(component).toBeTruthy();
  }));

});
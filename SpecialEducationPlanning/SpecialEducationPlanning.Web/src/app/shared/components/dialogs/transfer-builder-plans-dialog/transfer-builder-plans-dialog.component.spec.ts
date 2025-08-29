import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatMenuModule } from "@angular/material/menu";
import { MatRadioModule } from "@angular/material/radio";
import { MatTable } from "@angular/material/table";
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { NotificationsService } from "angular2-notifications";
import { ApiService } from "../../../../core/api/api.service";
import { ErrorLogService } from "../../../../core/services/error-log/error-log.service";
import { ServiceInjector } from "../../../../core/services/service-injector/service-injector";
import { ButtonComponent } from "../../atoms/button/button.component";
import { IconComponent } from "../../atoms/icon/icon.component";
import { InputComponent } from "../../atoms/input/input.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { RadioButtonComponent } from "../../atoms/radio-button/radio-button.component";
import { AutocompleteComponent } from "../../molecules/autocomplete/autocomplete.component";
import { SortMenuComponent } from "../../molecules/sort-menu/sort-menu.component";
import { TableComponent } from "../../organisms/table/table.component";
import { TableService } from "../../organisms/table/table.service";
import { TransferBuilderPlansDialogComponent } from "./transfer-builder-plans-dialog.component"


describe('TransferBuilderPlansDialogComponent', () => {

  let component: TransferBuilderPlansDialogComponent;
  let fixture: ComponentFixture<TransferBuilderPlansDialogComponent>;

  beforeEach(waitForAsync(() => {
    const testBed = TestBed.configureTestingModule({
      declarations: [TransferBuilderPlansDialogComponent, ModalComponent, IconComponent, ButtonComponent, InputComponent, AutocompleteComponent, TableComponent, MatTable, SortMenuComponent, RadioButtonComponent],
      imports: [CommonModule, TranslateModule.forRoot(), HttpClientModule, MatAutocompleteModule, RouterTestingModule, MatMenuModule, MatRadioModule],
      providers: [
        MatDialog,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
        { provide: ApiService, useValue: {} },
        { provide: NotificationsService, useValue: {} },
        { provide: ErrorLogService, useValue: {} },
        TableService
        
      ]
    })
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
    fixture = TestBed.createComponent(TransferBuilderPlansDialogComponent);
    component = fixture.debugElement.componentInstance;
  }));

  it('should create the component', waitForAsync(() => {
    expect(component).toBeTruthy();
  }));

  it('should have two elements with class .tdp-transfer-builder-plans-dialog-body-selectors-input', () => {
    const inputElement = fixture.debugElement.nativeElement.querySelectorAll('.tdp-transfer-builder-plans-dialog-body-selectors-input')
    expect(inputElement.length).toBe(2)
  });

  it('should have one autocomplete', () => {
    const inputElement = fixture.debugElement.nativeElement.querySelectorAll('tdp-autocomplete')
    expect(inputElement.length).toBe(1)
  });

});
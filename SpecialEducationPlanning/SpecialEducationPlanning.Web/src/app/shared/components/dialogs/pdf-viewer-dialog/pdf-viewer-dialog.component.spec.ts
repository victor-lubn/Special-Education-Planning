import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { ReactiveFormsModule } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { ApiService } from "../../../../core/api/api.service";
import { DownloadFileService } from "../../../../core/services/download-file/download-file.service";
import { ServiceInjector } from "../../../../core/services/service-injector/service-injector";
import { SharedModule } from "../../../shared.module";
import { ButtonComponent } from "../../atoms/button/button.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { PdfViewerDialogComponent } from "./pdf-viewer-dialog.component";

describe('PdfViewerDialogComponent', () => {

  let component: PdfViewerDialogComponent;
  let fixture: ComponentFixture<PdfViewerDialogComponent>;

  beforeEach(waitForAsync(() => {
      const testBed = TestBed.configureTestingModule({
          imports: [ SharedModule, ReactiveFormsModule, BrowserAnimationsModule, RouterTestingModule, 
              TranslateModule.forRoot(
              ), MatAutocompleteModule,
              HttpClientModule
          ],
          declarations: [
            PdfViewerDialogComponent, ButtonComponent, ModalComponent,
          ],
          providers: [
            { provide: ApiService, useValue: {} },
            { provide: MatDialog, useValue: {} },
            { provide: MatDialogRef, useValue: {} },
            { provide: MAT_DIALOG_DATA, useValue: {} },
            DownloadFileService
          ]
      })
      ServiceInjector.injector = testBed.get(Injector);
      testBed.compileComponents();
      fixture = TestBed.createComponent(PdfViewerDialogComponent);
      component = fixture.debugElement.componentInstance;
  }));

  it('should create', () => {
      expect(component).toBeTruthy();
    });
});
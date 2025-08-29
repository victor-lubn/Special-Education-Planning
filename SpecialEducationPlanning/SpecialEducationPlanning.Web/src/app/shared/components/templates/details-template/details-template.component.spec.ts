import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatRadioModule } from '@angular/material/radio';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute } from '@angular/router';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { Observable, Observer, of } from 'rxjs';
import { CommunicationService } from 'src/app/core/services/communication/communication.service';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { PermissionDirective } from 'src/app/shared/directives/permission.directive';
import { createTranslateLoader } from '../../../../app.module';
import { PlanService } from '../../../../core/api/plan/plan.service';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { Plan } from '../../../models/plan';
import { ButtonComponent } from '../../atoms/button/button.component';
import { CreateButtonComponent } from '../../atoms/create-button/create-button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { LabelComponent } from '../../atoms/label/label.component';
import { RadioButtonComponent } from '../../atoms/radio-button/radio-button.component';
import { TabsComponent } from '../../atoms/tabs/tabs.component';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { CustomerHeaderComponent } from '../../molecules/customer-header/customer-header.component';
import { CustomerInfoComponent } from '../../molecules/customer-info/customer-info.component';
import { CustomerNotesExpandedComponent } from '../../molecules/customer-notes/customer-notes-expanded/customer-notes-expanded.component';
import { CustomerNotesComponent } from '../../molecules/customer-notes/customer-notes.component';
import { SortMenuComponent } from '../../molecules/sort-menu/sort-menu.component';
import { CustomerContainerLeftHandSideComponent } from '../../organisms/customer-container-left-hand-side/customer-container-left-hand-side.component';
import { TableActionComponent } from '../../organisms/table/table-action/table-action.component';
import { TableActionsComponent } from '../../organisms/table/table-actions/table-actions.component';
import { TableMoreActionComponent } from '../../organisms/table/table-more-action/table-more-action.component';
import { TableMoreActionsComponent } from '../../organisms/table/table-more-actions/table-more-actions.component';
import { TableComponent } from '../../organisms/table/table.component';
import { DetailsTemplateComponent } from './details-template.component';

const fakePlansService:
  Partial<PlanService> = {
  getPlanTypes:
    jasmine.createSpy('getPlanTypes').and.returnValue(of([
      {
        value: 'Plan Type 1',
        key: 'plan type 1'
      },
      {
        value: 'Plan Type 2',
        key: 'plan type 2'
      },
      {
        value: 'Plan Type 3',
        key: 'plan type 3'
      },
    ])),
  getPlan: jasmine.createSpy('getPlan').and.returnValue(new Observable((observer: Observer<Partial<Plan>>) => {
    observer.next({
      builderTradingName: "Tom's Builders",
      cadFilePlanId: '33333',
      projectId: 222244,
    });
    observer.complete();
  })),
  getCatalogs: jasmine.createSpy('getCatalogs').and.returnValue(of([
    {
      code: '323134324',
      name: 'Catalogue 1'
    },
    {
      code: '32313433324',
      name: 'Catalogue 2'
    },
    {
      code: '32313114324',
      name: 'Catalogue 3'
    },
  ])),
};

const fakeUserInfoService: Partial<UserInfoService> = {
  hasPermission: jasmine.createSpy('hasPermission').and.returnValue(of(true))
};

const mockActivatedRoute = {
  params: of({ id: '2' })
};

describe('DetailsTemplateComponent', () => {
  let component: DetailsTemplateComponent;
  let fixture: ComponentFixture<DetailsTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        DetailsTemplateComponent, 
        ButtonComponent, 
        CustomerHeaderComponent,          
        IconComponent, 
        LabelComponent, 
        TableComponent, 
        TableActionsComponent, 
        TableActionComponent, 
        TableMoreActionsComponent, 
        TableMoreActionComponent, 
        SortMenuComponent, 
        RadioButtonComponent, 
        TabsComponent, 
        CustomerNotesComponent, 
        TextAreaComponent, 
        CustomerNotesExpandedComponent, 
        CustomerContainerLeftHandSideComponent, 
        CustomerInfoComponent, 
        CreateButtonComponent,
        PermissionDirective
      ],
      imports: [
        CommonModule, 
        MatTabsModule, 
        BrowserModule, 
        MatTableModule, 
        MatMenuModule, 
        MatRadioModule, 
        MatButtonModule, 
        MatPaginatorModule, 
        MatSortModule, 
        BrowserAnimationsModule, 
        FormsModule, 
        ReactiveFormsModule, 
        MatDialogModule, 
        MatAutocompleteModule, 
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }), 
        HttpClientModule
      ],
      providers: [
        { 
          provide: PlanService, 
          useValue: fakePlansService 
        },
        {
          provide: UserInfoService, 
          useValue: fakeUserInfoService
        },
        {
          provide: ActivatedRoute,
          useValue: mockActivatedRoute
        }, 
        TranslateService,
        CommunicationService,
        {
          provide: DialogsService,
          useValue: {}
        }
      ]
      })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailsTemplateComponent);
    component = fixture.componentInstance;
    component.ngOnInit();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

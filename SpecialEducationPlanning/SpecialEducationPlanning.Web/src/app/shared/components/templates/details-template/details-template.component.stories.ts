import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatRadioModule } from '@angular/material/radio';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { DetailsTemplateComponent } from './details-template.component';
import { ButtonComponent } from './../../atoms/button/button.component'
import { ButtonBackComponent } from '../../molecules/button-back/button-back.component';
import { IconComponent, iconNames } from '../../atoms/icon/icon.component';
import { LabelComponent } from '../../atoms/label/label.component';
import { RadioButtonComponent } from '../../atoms/radio-button/radio-button.component';
import { TabsComponent } from '../../atoms/tabs/tabs.component';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { CustomerNotesExpandedComponent } from '../../molecules/customer-notes/customer-notes-expanded/customer-notes-expanded.component';
import { CustomerNotesComponent } from '../../molecules/customer-notes/customer-notes.component';
import { SortMenuComponent } from '../../molecules/sort-menu/sort-menu.component';
import { CustomerContainerLeftHandSideComponent } from '../../organisms/customer-container-left-hand-side/customer-container-left-hand-side.component';
import { TableActionComponent } from '../../organisms/table/table-action/table-action.component';
import { TableActionsComponent } from '../../organisms/table/table-actions/table-actions.component';
import { TableComponent } from '../../organisms/table/table.component';
import { TableService } from '../../organisms/table/table.service';
import { CustomerHeaderComponent } from './../../molecules/customer-header/customer-header.component';
import { TableMoreActionComponent } from './../../organisms/table/table-more-action/table-more-action.component';
import { TableMoreActionsComponent } from './../../organisms/table/table-more-actions/table-more-actions.component';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../app.module';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CustomerInfoComponent } from '../../molecules/customer-info/customer-info.component';
import { CreateButtonComponent } from '../../atoms/create-button/create-button.component';

const customerData = {
  tradingName: 'Trading Name',
  address0: 'Address One',
  address1: 'Address Two',
  address2: 'Address Three',
  address3: 'Address Four',
  postcode: 'Postcode',
  landLineNumber: '01604 654987',
  mobileNumber: '07780 564321',
  email: 'lee.bishop@aiep.com',
}

export default {
  component: DetailsTemplateComponent,
  decorators: [
    moduleMetadata({
      declarations: [DetailsTemplateComponent, ButtonComponent, CustomerHeaderComponent, ButtonBackComponent, IconComponent, LabelComponent, TableComponent, TableActionsComponent, TableActionComponent, TableMoreActionsComponent, TableMoreActionComponent, SortMenuComponent, RadioButtonComponent, TabsComponent, CustomerNotesComponent, TextAreaComponent, CustomerNotesExpandedComponent, CustomerContainerLeftHandSideComponent, CustomerInfoComponent, CreateButtonComponent],
      imports: [CommonModule, MatTabsModule, BrowserModule, MatTableModule, MatMenuModule, MatRadioModule, MatButtonModule, MatPaginatorModule, MatSortModule, BrowserAnimationsModule, FormsModule, ReactiveFormsModule, MatDialogModule, MatAutocompleteModule, TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: (createTranslateLoader),
          deps: [HttpClient]
        }
      }), HttpClientModule],
      providers: [TableService, { provide: MAT_TAB_GROUP, useValue: {} }, { provide: MAT_DIALOG_DATA, useValue: {} }, TranslateService]
    })
  ],
  argTypes: {
  },
  title: 'Template/Details Template'
} as Meta;

const Template: Story<DetailsTemplateComponent> = (args) => ({
  props: {
    ...args,
  },
  template:
    `
    <tdp-details-template>
      <div class="details-header">
      <tdp-customer-header
      [builderName]="builderName"
      [accountTitle]="accountTitle"
      [creditAccount]="creditAccount"
      [label]="label"
      ></tdp-customer-header>
      </div>

      <div class="details-left-sidebar">
      <tdp-customer-container-left-hand-side
        [customerData]="customerData"
    ></tdp-customer-container-left-hand-side>
      <tdp-confirmation
      [title]="title"
      [description]="description"
      [cancelation]="cancelation"
      [confirmation]="confirmation"
    ></tdp-confirmation>


      </div>

      <div class="details-top-container">
        <h2>Customer notes</h2>
        <tdp-customer-notes [noteValue]=""  [placeholder]="'plan.makeNote' | translate"></tdp-customer-notes>
     </div>


      <div class="details-bottom-container">
        <tdp-tabs>
        <mat-tab label="Active Plans" class="plan-tab">
          <tdp-table [records]="records" [columnsConfiguration]="activePlans">
              <tdp-table-actions>
                <ng-template let-record="record">
                  <tdp-table-action (click)="onClick(record)">
                    <tdp-icon [size]="iconSize" [name]="iconPreview"></tdp-icon>
                  </tdp-table-action>
                  <tdp-table-action (click)="onClick(record)">
                    <tdp-icon [size]="iconSize" [name]="iconOpen"></tdp-icon>
                  </tdp-table-action>
                  <tdp-table-more-actions>
                    <tdp-table-more-action (click)="onClick(record)">
                      Transfer Plan
                    </tdp-table-more-action>
                  </tdp-table-more-actions>
                </ng-template>
              </tdp-table-actions>
            </tdp-table>
        </mat-tab>
        <mat-tab label="Archived Plans" class="plan-tab">
          <tdp-table [records]="records" [columnsConfiguration]="activePlans">
              <tdp-table-actions>
                <ng-template let-record="record">
                  <tdp-table-action (click)="onClick(record)">
                    <tdp-icon [size]="iconSize" [name]="iconPreview"></tdp-icon>
                  </tdp-table-action>
                  <tdp-table-action (click)="onClick(record)">
                    <tdp-icon [size]="iconSize" [name]="iconOpen"></tdp-icon>
                  </tdp-table-action>
                  <tdp-table-more-actions>
                    <tdp-table-more-action (click)="onClick(record)">
                      Transfer Plan
                    </tdp-table-more-action>
                  </tdp-table-more-actions>
                </ng-template>
              </tdp-table-actions>
            </tdp-table>
        </mat-tab>
        </tdp-tabs>
      </div>

    </tdp-details-template>
    `
});

export const Details = Template.bind({});
Details.args = {
  customerData: {
    tradingName: 'Trading Name',
    address0: 'Address One',
    address1: 'Address Two',
    address2: 'Address Three',
    address3: 'Address Four',
    postcode: 'Postcode',
    landLineNumber: '01604 654987',
    mobileNumber: '07780 564321',
    email: 'lee.bishop@aiep.com',
  },
  builderName: 'Bishops Builders',
  accountTitle: 'Account',
  creditAccount: '4563765390',
  label: 'credit account',
  title: "Sign out",
  description: "Are you sure?",
  cancelation: "NO",
  confirmation: "YES",
  iconSize: '24px',
  iconPreview: iconNames.size24px.PREVIEW,
  iconOpen: iconNames.size24px.LAUNCH_BACK,
  iconOptions: iconNames.size24px.MORE_VERTICAL,
  records: {
    "take": 20,
    "skip": 0,
    "total": 6,
    "data": [
      {
        "id": 1,
        "title": null,
        "lastOpen": "2019-03-24T11:29:23.768",
        "projectId": 1,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567891",
        "planName": "Test plan b",
        "catalogId": 1,
        "endUserId": null,
        "endUser": {
          "fullName": null
        },
        "builderId": 1,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ11TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 1,
        "isStarred": false,
        "builderTradingName": "Cash Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 2,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan a",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 3,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan d",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 4,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan f",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 5,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan e",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 6,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan g",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      }
    ]
  },
  activePlans: [
    { columnDef: 'planCode', header: 'Plan ID', sortField: 'planCode' },
    { columnDef: 'endUser', header: 'End user', field: 'endUser.fullName', sortField: 'endUser' },
    { columnDef: 'Educationer', header: 'Educationer', callback: (record: any) => `${record.Educationer?.firstName} ${record.Educationer?.surname}`, sortField: 'Educationer' },
    { columnDef: 'masterVersionId', header: 'Version ID', sortField: 'masterVersionId' },
    { columnDef: 'updatedDate', header: 'Last Updated', sortField: 'updatedDate' },
  ],
}


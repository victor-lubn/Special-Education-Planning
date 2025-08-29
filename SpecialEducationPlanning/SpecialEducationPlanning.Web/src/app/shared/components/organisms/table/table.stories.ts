import { TableComponent } from './table.component';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LabelComponent } from '../../atoms/label/label.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { TableActionsComponent } from './table-actions/table-actions.component';
import { TableActionComponent } from './table-action/table-action.component';
import { iconNames } from '../../atoms/icon/icon.component';
import { action } from '@storybook/addon-actions';
import { TableService } from './table.service';
import { SortMenuComponent } from '../../molecules/sort-menu/sort-menu.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { RadioButtonComponent } from '../../atoms/radio-button/radio-button.component';
import { MatRadioModule } from '@angular/material/radio';
import { PipeModule } from '../../../pipes/pipes.module';

export default {
  component: TableComponent,
  subcomponents: { TableActionsComponent, IconComponent },
  decorators: [
    moduleMetadata({
      declarations: [TableComponent, TableActionsComponent, TableActionComponent, SortMenuComponent, RadioButtonComponent, IconComponent, LabelComponent],
      imports: [CommonModule, MatTableModule, MatMenuModule, MatRadioModule, MatButtonModule, MatPaginatorModule, MatSortModule, BrowserAnimationsModule, PipeModule],
      providers: [TableService]
    }),
  ],
  argsTypes : {
    onClick: {
      action: 'clicked'
    }
  },
  title: 'Organism/Table',
} as Meta;



const Template: Story = args => ({
  props: {
    ...args,
    onClick: action('Clicked button!')
  },
  template: args.template,
});

export const Table = Template.bind({});
Table.args = {
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
  columnsConfiguration: [
    { columnDef: 'id', header: 'ID', sortField: 'id' },
    { columnDef: 'planName', header: 'Plan name', sortField: 'planName' },
    { columnDef: 'endUser', header: 'End user', field: 'endUser.fullName', sortField: 'endUser' },
    { columnDef: 'Educationer', header: 'Educationer', callback: (record: any) => `${record.Educationer?.firstName} ${record.Educationer?.surname}`, sortField: 'Educationer' },
    { columnDef: 'lastOpen', custom: true },
  ],
  size: '24px',
  user: iconNames.size24px.ACCOUNT_CIRCLE,
  archive: iconNames.size24px.ARCHIVE,
  template: `
  <tdp-table [records]="records" [columnsConfiguration]="columnsConfiguration">
    <ng-container matColumnDef="lastOpen">
      <mat-header-cell *matHeaderCellDef>Last open</mat-header-cell>
      <mat-cell *matCellDef="let record">
        <tdp-label>{{ record.lastOpen | date: 'dd/MM/yyyy' }} </tdp-label>
      </mat-cell>
    </ng-container>

    <tdp-table-actions>
      <ng-template let-record="record">
        <tdp-table-action (click)="onClick(record)">
          <tdp-icon [size]="size" [name]="user"></tdp-icon>
        </tdp-table-action>
        <tdp-table-action (click)="onClick(record)">
          <tdp-icon [size]="size" [name]="archive"></tdp-icon>
        </tdp-table-action>
        <tdp-table-action (click)="onClick(record)">
          <tdp-icon [size]="size" [name]="archive"></tdp-icon>
        </tdp-table-action>
      </ng-template>
    </tdp-table-actions>
  </tdp-table>
`
};

export const AssignTable = Template.bind({});
AssignTable.args = {
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
  columnsConfiguration: [
    { columnDef: 'planCode', header: 'Plan ID', sortField: 'planCode' },
    { columnDef: 'endUser', header: 'End user', field: 'endUser.fullName', sortField: 'endUser' },
    { columnDef: 'Educationer', header: 'Educationer', callback: (record: any) => `${record.Educationer?.firstName} ${record.Educationer?.surname}`, sortField: 'Educationer' },
    { columnDef: 'updatedDate', custom: true }
  ],
  size: '24px',
  selectable: true,
  hasActions: false,
  template: `
  <tdp-table [records]="records" [columnsConfiguration]="columnsConfiguration" [hasActions]="hasActions" [selectable]="selectable">
    <ng-container matColumnDef="updatedDate">
      <mat-header-cell *matHeaderCellDef>Last Updated</mat-header-cell>
      <mat-cell *matCellDef="let record">
        {{ record.updatedDate | date: 'dd/MM/yy' }}
      </mat-cell>
    </ng-container>
  </tdp-table>
`
};

export const TradeCustomerTable = Template.bind({});
TradeCustomerTable.args = {
  records: {
    "take": 20,
    "skip": 0,
    "total": 4,
    "data": [
      {
        accountNumber: "0008000001",
        address0: "3 Earl Lane Dublin",
        address1: "3 Earl Lane",
        address2: null,
        address3: "Dublin",
        builderEducationerAieps: [],
        createdDate: "2021-11-16T10:37:47.79583",
        creationUser: "DZ99TDP.Support@hwdn.co.uk",
        email: null,
        id: 3,
        landLineNumber: "123456789",
        mobileNumber: "07790436393",
        name: null,
        notes: null,
        owningAiepCode: "8001",
        owningAiepName: "Ballyfermot",
        plans: [],
        postcode: "D01HM90",
        sapAccountStatus: null,
        tradingName: "6030 IE Test Customer",
        updateUser: "DZ99TDP.Support@hwdn.co.uk",
        updatedDate: "2021-11-16T10:37:47.79583"
      },
      {
        accountNumber: "0008000002",
        address0: "5 Mihai Viteazu",
        address1: "5 Mihai Viteazul Cartier",
        address2: null,
        address3: "Cluj-Napoca",
        builderEducationerAieps: [],
        createdDate: "2021-11-16T10:37:47.79583",
        creationUser: "DZ99TDP.Support@hwdn.co.uk",
        email: null,
        id: 4,
        landLineNumber: "123456789",
        mobileNumber: "0770353000",
        name: null,
        notes: null,
        owningAiepCode: "8002",
        owningAiepName: "Cluj-Napoca",
        plans: [],
        postcode: "400105",
        sapAccountStatus: null,
        tradingName: "Cluj Trading Name",
        updateUser: "DZ99TDP.Support@hwdn.co.uk",
        updatedDate: "2021-11-16T10:37:47.79583"
      },
      {
        accountNumber: "0008000003",
        address0: "5 Corn's Oradea",
        address1: "5 Corn's",
        address2: null,
        address3: "Oradea",
        builderEducationerAieps: [],
        createdDate: "2021-11-16T10:37:47.79583",
        creationUser: "DZ99TDP.Support@hwdn.co.uk",
        email: null,
        id: 5,
        landLineNumber: "123456789",
        mobileNumber: "0762626626",
        name: null,
        notes: null,
        owningAiepCode: "8003",
        owningAiepName: "Oradea Aiep",
        plans: [],
        postcode: "D01HM90",
        sapAccountStatus: null,
        tradingName: "Oradea S Trade Customer",
        updateUser: "DZ99TDP.Support@hwdn.co.uk",
        updatedDate: "2021-11-16T10:37:47.79583"
      },
      {
        accountNumber: "0008000004",
        address0: "4B Cartier SebeИ™",
        address1: "4B Cartier VF SebeИ™",
        address2: null,
        address3: "SebeИ™",
        builderEducationerAieps: [],
        createdDate: "2021-11-16T10:37:47.79583",
        creationUser: "DZ99TDP.Support@hwdn.co.uk",
        email: null,
        id: 5,
        landLineNumber: "123456789",
        mobileNumber: "0776565685",
        name: null,
        notes: null,
        owningAiepCode: "8003",
        owningAiepName: "London Aiep",
        plans: [],
        postcode: "515800",
        sapAccountStatus: null,
        tradingName: "SebeИ™ Trade Customer",
        updateUser: "DZ99TDP.Support@hwdn.co.uk",
        updatedDate: "2021-11-16T10:37:47.79583"
      },
    ]
  },
  columnsConfiguration: [
    { columnDef: 'tradingName', header: 'Trading Name', field: 'tradingName', sortField: 'tradingName' },
    { columnDef: 'address', header: 'Address', field: 'address0', sortField: 'address0' },
    { columnDef: 'postcode', header: 'Postcode', field: 'postcode', sortField: 'postcode' },
    { columnDef: 'mobileNumber', header: 'Mobile Number', field: 'mobileNumber', sortField: 'mobileNumber' },
  ],
  selectable: true,
  hasActions: false,
  template: `
  <tdp-table
   [records]="records"
   [columnsConfiguration]="columnsConfiguration"
   [hasActions]="hasActions"
   [selectable]="selectable">
  </tdp-table>
  `
}



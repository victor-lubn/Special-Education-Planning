import { animate, keyframes, style, transition, trigger } from '@angular/animations';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ApiService } from 'src/app/core/api/api.service';
import { Builder } from 'src/app/shared/models/builder';
import { ValidationBuilderResponse } from 'src/app/shared/models/validation-builder-response';
import { FormComponent } from '../../../base-classes/form-component';
import { TradeCustomerSearchTypeEnum } from '../../../models/app-enums';
import { SimpleInformationDialogComponent } from '../../dialogs/simple-information-dialog/simple-information-dialog.component';

@Component({
  selector: 'tdp-account-look-up',
  templateUrl: './account-look-up.component.html',
  styleUrls: ['./account-look-up.component.scss'],
  animations: [
    trigger('fadeAnimation', [
      transition(
        ':enter', [
        style({ opacity: 0 }),
        animate('1s ease-out', style({ opacity: 1 }))
      ])
    ]),
    trigger('slideAnimation', [
      transition(
        ':enter', [
        animate('1s ease-in', keyframes([
          style({ opacity: 0, transform: 'translateY(-50%)', offset: 0 }),
          style({ opacity: 0.2, transform: 'translateY(-25%)', offset: 0.2 }),
          style({ opacity: 1, transform: 'translateY(0%)', offset: 1 }),
        ])),
      ]),
      transition(
        ':leave', [
        animate('0.5s ease-out', keyframes([
          style({ opacity: 1, transform: 'translateY(0%)', offset: 0 }),
          style({ opacity: 0.2, transform: 'translateY(-25%)', offset: 0.2 }),
          style({ opacity: 0, transform: 'translateY(-50%)', offset: 1 }),
        ])),
      ])
    ])
  ]
})
export class AccountLookUpComponent extends FormComponent implements OnInit {

  @Output()
  cancelEmitter = new EventEmitter<void>();

  @Output()
  isConfirmedEmitter = new EventEmitter<boolean>();

  isChecking: boolean = true;
  isConfirmed: boolean = false;
  tradeCustomer: Builder;
  searchType: TradeCustomerSearchTypeEnum;

  private readonly maxAccountNumberLength: number = 11;

  constructor(
    private api: ApiService,
    private matDialog: MatDialog
  ) {
    super();
    this.entityForm = this.formBuilder.group({
      accountNumber: [null, Validators.maxLength(this.maxAccountNumberLength)]
    });
  }

  ngOnInit(): void {
  }

  onCheck(): void {
    if (!this.entityForm.value.accountNumber) {
      this.matDialog.open(SimpleInformationDialogComponent, {
        data: {
          titleStringKey: 'dialog.builderAccountNumberNotEntered.title',
          messageStringKey: 'dialog.builderAccountNumberNotEntered.message'
        }
      })
    } else { 
      const accountNumberSubscription = this.api.builders.validatePossibleMatchingBuildersByAccountNumber(
        this.entityForm.value.accountNumber
      )
        .subscribe((validationResponse: ValidationBuilderResponse) => {
          if (validationResponse) {
            this.isChecking = false;
            this.tradeCustomer = validationResponse.builders[0].builder;
            this.searchType = validationResponse.builders[0].builderSearchType;
          } else {
            this.entityForm.reset();
            this.matDialog.open(SimpleInformationDialogComponent, {
              data: {
                titleStringKey: 'dialog.builderAccountNumberNotFound.title',
                messageStringKey: 'dialog.builderAccountNumberNotFound.message'
              }
            })
          }
        });
      this.entitySubscriptions.push(accountNumberSubscription);
    }
  }

  onCancel(): void {
    this.changeIsConfirmed(false);
    this.isChecking = true;
    this.cancelEmitter.emit();
  }

  onConfirm(): void {
    this.changeIsConfirmed(true);
  }

  changeIsConfirmed(value: boolean): void {
    this.isConfirmed = value;
    this.isConfirmedEmitter.emit(this.isConfirmed);
  }

}

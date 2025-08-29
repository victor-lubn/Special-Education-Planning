import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, AbstractControl, ValidatorFn } from '@angular/forms';

import { ServiceInjector } from '../../core/services/service-injector/service-injector';
import { BaseComponent } from './base-component';

export abstract class FormComponent extends BaseComponent {

  public entityForm: UntypedFormGroup;
  public editing: boolean;

  protected formBuilder: UntypedFormBuilder;

  constructor() {
    super();
    this.editing = false;
    this.formBuilder = ServiceInjector.injector.get(UntypedFormBuilder);
    this.entityForm = this.formBuilder.group({});
  }

  public enableEdit(): void {
    this.editing = true;
    this.entityForm.enable();
  }

  public cancelEdit(originalEntityValue: any): void {
    this.editing = false;
    if (originalEntityValue) {
      this.entityForm.patchValue(originalEntityValue);
      this.entityForm.markAsPristine();
    } else {
      this.entityForm.reset();
    }
    this.entityForm.disable();
  }

  /**
   * @deprecated Extend FormComponent class and use checkHasErrorV2() instead of checkHasError()
   */
  public checkHasError(form: UntypedFormGroup, formControlName: string, errorKey?: string): boolean {
    return this.hasFormControlError(form.get(formControlName) as UntypedFormControl, errorKey);
  }

  public checkHasErrorV2(formControlName: string, errorKey?: string): boolean {
    return this.hasFormControlError(this.entityForm.get(formControlName) as UntypedFormControl, errorKey);
  }

  protected addValidators(formControl: AbstractControl, validatorList: ValidatorFn | ValidatorFn[]): void {
    formControl.setValidators(validatorList);
    formControl.updateValueAndValidity({
      onlySelf: true,
      emitEvent: false
    });
  }

  protected markFormGroupTouched(formGroup: UntypedFormGroup): void {
    (<any>Object).values(formGroup.controls).forEach((control) => {
      control.markAsTouched();
      if (control.controls) {
        this.markFormGroupTouched(control);
      }
    });
  }

  protected markFormGroupDirty(formGroup: UntypedFormGroup): void {
    (<any>Object).values(formGroup.controls).forEach((control) => {
      control.markAsDirty();
      if (control.controls) {
        this.markFormGroupDirty(control);
      }
    });
  }

  private hasFormControlError(formControl: UntypedFormControl, errorKey?: string): boolean {
    if (formControl.status === 'DISABLED') {
      return false;
    }
    if (errorKey) {
      return (formControl.touched || formControl.dirty) && !formControl.valid && formControl.errors && !!(formControl.errors[errorKey]);
    } else {
      return (formControl.touched || formControl.dirty) && !formControl.valid;
    }
  } 

}

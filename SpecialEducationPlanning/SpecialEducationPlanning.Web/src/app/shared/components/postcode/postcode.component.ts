import { Component, EventEmitter, forwardRef, Input, Output, Renderer2, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { Subject } from 'rxjs';
import { debounceTime, finalize } from 'rxjs/operators';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';
import { ApiService } from '../../../core/api/api.service';
import { Suggestions } from '../../../shared/models/address';
import { BaseEntity } from '../../base-classes/base-entity';

export const POSTCODE_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => PostcodeComponent),
  multi: true
};

@Component({
  selector: 'tdp-postcode',
  providers: [POSTCODE_VALUE_ACCESSOR],
  templateUrl: 'postcode.component.html',
  styleUrls: ['postcode.component.scss']
})
export class PostcodeComponent extends BaseEntity implements ControlValueAccessor {

  public suggestions: Suggestions[];
  public searching: boolean;

  @Input()
  hasError: boolean;

  @Input()
  public reverse = false;

  @Input()
  public isAddress1 = false;

  @Input()
  public selectedCountry: string;

  @Output()
  public selectedAddress = new EventEmitter<any>();

  @ViewChild('postcodeInput', { static: true })
  private input;

  @Input()
  public postcode?: string;

  private onChange: any;
  private onTouch: any;
  private debouncer: Subject<string> = new Subject<string>();
  private postcodePipe;
  private countryService;
  private minCharacter: number;
  public currentValue: string;

  constructor(
    private notifications: NotificationsService,
    private translateSvc: TranslateService,
    private renderer: Renderer2,
    private api: ApiService,
    public country: CountryControllerService,
  ) {
    super();
    this.currentValue = "";
    this.minCharacter = 3;
    this.suggestions = [];
    this.searching = false;
    const subscription = this.debouncer.pipe(debounceTime(1000)).subscribe((value) => {
      const valueTrim = (value ?? '').trim();
      if (valueTrim.length >= this.minCharacter) {
        if (this.reverse) {
          this.searchAddressesByPostcode(valueTrim);
        } else {
          this.searchAddressesByAddress1(valueTrim);
        }
      } else {
        this.suggestions = [];
        if(value.length > this.minCharacter) {
          this.notifications.warn(this.translateSvc.instant('notification.emptySearchWarning'));
        }
      }
    });
    this.entitySubscriptions.push(subscription);
    this.countryService = this.country.getService();
    this.postcodePipe = this.countryService.getPostCodeTransform();
  }

  //This function sets the postcode field to display nothing between selecting the option, and retrieving the data from the backend
  displayEmpty(option) {
    return null;
  }

  public selectedPostcodeOption(event?: MatAutocompleteSelectedEvent) {
    this.searching = true;
    this.currentValue = encodeURIComponent(event.option.value.format);
    const subscribe = this.api.postcodes.getAddresByUri(this.currentValue, this.selectedCountry)
      .pipe(finalize(() => {
        this.searching = false;
      }))
      .subscribe((response) => {
        if (response) {
          this.selectedAddress.emit(response);
        }
      })
    this.entitySubscriptions.push(subscribe);
  }

  public writeValue(value: string): void {
    if (!this.isAddress1) {
      this.renderer.setProperty(this.input.nativeElement, 'value', this.postcodePipe.transform(value, true));
    } else {
      this.renderer.setProperty(this.input.nativeElement, 'value', value);
    }
  }

  public registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  public inputChange($event): void {
    this.onChange($event.target.value);

    if (this.isAddress1) {
    this.debouncer.next($event.target.value + " " + this.postcode);
    }
    else {
      this.debouncer.next($event.target.value);
    }
  }

  public registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }

  public inputTouched(): void {
    this.onTouch();
  }

  public setDisabledState?(isDisabled: boolean): void {
    this.renderer.setProperty(this.input.nativeElement, 'disabled', isDisabled);
  }

  private searchAddressesByPostcode(postcode: string): void {
    if (postcode) {
      this.searching = true;
      const addressesSubscription = this.api.postcodes.getAddressSuggestionsByPostcode(postcode, this.selectedCountry)
        .pipe(finalize(() => {
          this.searching = false;
        }))
        .subscribe((response) => {
          this.suggestions = response;
        }, (error) => {
          this.suggestions = [];
        });
      this.entitySubscriptions.push(addressesSubscription);
    } else {
      this.suggestions = [];
    }
  }

  private searchAddressesByAddress1(address1: string): void {
    if (address1) {
      this.searching = true;
      const addressesSubscription = this.api.postcodes.getAddressSuggestionsByAddress(address1, this.selectedCountry)
        .pipe(finalize(() => {
          this.searching = false;
        }))
        .subscribe((response) => {
          this.suggestions = response;
        }, (error) => {
          this.suggestions = [];
        });
      this.entitySubscriptions.push(addressesSubscription);
    } else {
      this.suggestions = [];
    }
  }
}

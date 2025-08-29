import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { UntypedFormGroup, Validators } from "@angular/forms";
import { TranslateService } from "@ngx-translate/core";
import { ApiService } from "src/app/core/api/api.service";
import { FormComponent } from "src/app/shared/base-classes/form-component";
import { EndUser } from "src/app/shared/models/end-user";
import { Title } from "src/app/shared/models/title.model";
import { getPostcodeValidator } from "../../../../../validators/control-validators/postcode";

@Component({
    selector: 'tdp-end-user-form-dialog',
    templateUrl: './end-user-form-dialog.component.html',
    styleUrls: ['./end-user-form-dialog.component.scss']
})
export class EndUserFormDialogComponent extends FormComponent implements OnInit, OnChanges {

    @Input()
    public endUser: EndUser;

    @Output()
    public formStatusChanges = new EventEmitter();

    @Output()
    public endUserSubmitted = new EventEmitter<UntypedFormGroup>();

    public endUserTitles;

    public titleString: string;
    public firstNameString: string;
    public surnameString: string;
    public emailString: string;
    public mobileNumberString: string;
    public landlineNumberString: string;
    public postcodeString: string;
    public address1String: string;
    public address2String: string;
    public address3String: string;
    public notesString: string;
    public notesPlaceholderString: string;

    public readonly postcodeMaxLength: number = 30;
    public readonly addressMaxLength: number = 100;
    public readonly notesMaxLength: number = 500;

    constructor(
        private api: ApiService,
        private translate: TranslateService,
        private cdr: ChangeDetectorRef
    ) {
        super();
        this.endUserTitles = [];
    }

    ngOnInit(): void {        
        this.initializeEndUserForm();
        this.initializeTitleSelector();
        this.initializeTranslations();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.endUser && changes.endUser.currentValue) {
            this.initializeEndUserForm();
            this.cdr.detectChanges();
        }
    }

    public displayEndUserTitles() {
        return [];
    }
    
    public submitEndUserForm() {
        if (this.entityForm.valid) {
            this.endUserSubmitted.emit(this.entityForm.value);
        } else {
            this.entityForm.markAllAsTouched();
        }
    }

    public onUpdateComment(comment) {
        this.entityForm.get('comment').setValue(comment.note);
    }

    private initializeEndUserForm() {
        if (!this.endUser) {
            this.entityForm = this.formBuilder.group({
                titleId: [null],
                firstName: [null],
                surname: [null, Validators.required],
                contactEmail: [null, Validators.email],
                mobileNumber: [null, Validators.pattern('[- +()0-9]+')],
                landLineNumber: [null, Validators.pattern('[- +()0-9]+')],
                postcode: [null, [Validators.required, Validators.maxLength(30), this.getPostcodeValidation()]],
                address1: [null, [Validators.required, Validators.maxLength(100)]],
                address2: [null, Validators.maxLength(100)],
                address3: [null, Validators.maxLength(100)],
                comment: [null, Validators.maxLength(this.notesMaxLength)]
            });
        } else {
            this.entityForm = this.formBuilder.group({
                titleId: [this.endUser.titleId],
                firstName: [this.endUser.firstName],
                surname: [this.endUser.surname, Validators.required],
                contactEmail: [this.endUser.contactEmail, Validators.email],
                mobileNumber: [this.endUser.mobileNumber, Validators.pattern('[- +()0-9]+')],
                landLineNumber: [this.endUser.landLineNumber, Validators.pattern('[- +()0-9]+')],
                postcode: [this.endUser.postcode, [Validators.required, Validators.maxLength(30), this.getPostcodeValidation()]],
                address1: [this.endUser.address1, [Validators.required, Validators.maxLength(100)]],
                address2: [this.endUser.address2, Validators.maxLength(100)],
                address3: [this.endUser.address3, Validators.maxLength(100)],
                comment: [this.endUser.comment, Validators.maxLength(this.notesMaxLength)]
            });
        }
    }

    private initializeTitleSelector() {
        const endUserTitlesSubscription = this.api.plans
            .getEndUserTitles()
            .subscribe((titles: Title[]) => {
                this.endUserTitles = titles.map((title: Title) => {
                    return {
                        value: title.id,
                        text: title.titleName
                    };
                });
            });
        this.entitySubscriptions.push(endUserTitlesSubscription);
    }

    private getPostcodeValidation() {
        return getPostcodeValidator().getValidator;
      }

    private initializeTranslations() {
        const translationSubscription = this.translate.get([
            'endUser.formTitle',
            'endUser.firstName',
            'endUser.surname',
            'endUser.email',
            'endUser.mobileNumber',
            'endUser.landlineNumber',
            'endUser.postcode',
            'endUser.address1',
            'endUser.address2',
            'endUser.address3',
            'endUser.notes',
            'endUser.notesPlaceholder'
        ]).subscribe((translations) => {
            this.titleString = translations['endUser.formTitle'];
            this.firstNameString = translations['endUser.firstName'];
            this.surnameString = translations['endUser.surname'];
            this.emailString = translations['endUser.email'];
            this.mobileNumberString = translations['endUser.mobileNumber'];
            this.landlineNumberString = translations['endUser.landlineNumber'];
            this.postcodeString = translations['endUser.postcode'];
            this.address1String = translations['endUser.address1'];
            this.address2String = translations['endUser.address2'];
            this.address3String = translations['endUser.address3'];
            this.notesString = translations['endUser.notes'];
            this.notesPlaceholderString = translations['endUser.notesPlaceholder'];
        });
        this.entitySubscriptions.push(translationSubscription);
    }

}
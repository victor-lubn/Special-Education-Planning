import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormControl, UntypedFormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from "@ngx-translate/core";
import { ErrorLogService } from "src/app/core/services/error-log/error-log.service";
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { IconComponent } from "../../atoms/icon/icon.component";
import { InputComponent } from "../../atoms/input/input.component";
import { SelectComponent, SelectOptionInterface } from "../../atoms/select/select.component";
import { FilterHeaderComponent } from './filter-header.component';

describe('FilterHeaderComponent', () => {
  let component: FilterHeaderComponent;
  let fixture: ComponentFixture<FilterHeaderComponent>;
  let options: SelectOptionInterface[] = [];

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [FilterHeaderComponent, InputComponent, SelectComponent, IconComponent],
      imports: [ReactiveFormsModule, RouterTestingModule, CommonModule, FormsModule, TranslateModule.forRoot()],
      providers: [{ provide: ErrorLogService, useValue: {} }, HttpClientModule]
    })
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
    fixture = TestBed.createComponent(FilterHeaderComponent);
    component = fixture.componentInstance;
    component.form = new UntypedFormGroup({
      filterBy: new UntypedFormControl(null),
      search: new UntypedFormControl(null)
    })
    component.options = options;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ChangeDetectorRef, QueryList } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonFilterComponent, ButtonFilterGroup } from './button-filter.component';

describe('ButtonFilterComponent', () => {
  let component: ButtonFilterComponent;
  let changeDetectorService: ChangeDetectorRef;
  let fixture: ComponentFixture<ButtonFilterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ButtonFilterGroup, ButtonFilterComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonFilterComponent);
    component = fixture.componentInstance;

    changeDetectorService = fixture.debugElement.injector.get(ChangeDetectorRef)
    component.buttonFilterGroup = new ButtonFilterGroup(changeDetectorService);
    component.buttonFilterGroup._buttonFilters = new QueryList();
    component.buttonFilterGroup._buttonFilters.reset([component, component, component]);
    component.buttonFilterGroup.ngOnInit();
    component.buttonFilterGroup.ngAfterContentInit();

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have the default values', () => {
    expect(component.icon).toBe(undefined);
    expect(component.tag).toBe(undefined);
    expect(component.number).toBe(0);
    expect(component.value).toBe(undefined);
  });

  it('should have checked value', () => {
    component.buttonFilterGroup._buttonFilters.forEach((filter) => {
      filter.value = 1;
      filter.checked = true;
    });
    fixture.detectChanges();
    expect(component.value).toBe(1);
    expect(component.checked).toBeTrue();
  });

  it('should select filter', () => {
    component.handleOnClick();
    fixture.detectChanges();
    expect(component.checked).toBeTrue();
  });

  it('should select all checked when multiple', () => {
    component.buttonFilterGroup.multiple = true;
    component.buttonFilterGroup._buttonFilters.forEach((filter, index) => {
      filter.value = index + 1;
      filter.checked = false;
    });
    component.handleOnClick();
    fixture.detectChanges();
    expect(component.buttonFilterGroup.value).toEqual([3]);
  });

  it('should check by default when value defined', () => {
    component.buttonFilterGroup.writeValue(3);
    component.buttonFilterGroup._buttonFilters.forEach((filter, index) => {
      filter.value = index + 1;
      filter.checked = false;
    });
    component.buttonFilterGroup.ngOnInit();
    fixture.detectChanges();
    component.ngOnInit();
    fixture.detectChanges();
    expect(component.buttonFilterGroup._buttonFilters.find((filter) => filter.checked)).toBeDefined();
  });

  it('should have a method for registeronchange', () => {
    expect(typeof component.buttonFilterGroup.registerOnChange).toBe('function');
    let called = false;
    component.buttonFilterGroup.registerOnChange((value) => called = value);
    component.buttonFilterGroup._controlValueAccessorChangeFn(true);
    expect(called).toBeTrue();
  })

  it('should have a method for registerontouched', () => {
    expect(typeof component.buttonFilterGroup.registerOnTouched).toBe('function');
    let called = false;
    component.buttonFilterGroup.registerOnTouched(() => called = true);
    component.buttonFilterGroup._onTouched();
    expect(called).toBeTrue();
  })
});

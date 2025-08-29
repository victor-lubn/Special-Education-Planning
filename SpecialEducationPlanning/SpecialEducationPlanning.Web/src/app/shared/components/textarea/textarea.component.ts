import { Component, Renderer2, ViewChild, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export const TEXTAREA_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => TextareaComponent),
  multi: true
};

@Component({
  selector: 'tdp-textarea',
  providers: [TEXTAREA_VALUE_ACCESSOR],
  templateUrl: 'textarea.component.html',
  styleUrls: ['textarea.component.scss']
})
export class TextareaComponent implements ControlValueAccessor {

  @Input()
  set outlined(outlined: boolean) {
    if (!outlined) {
      this.renderer.addClass(this.textarea.nativeElement, 'no-outline');
    }
  }

  @ViewChild('textarea', { static: true })
  private textarea;

  private onChange: any;
  private onTouch: any;

  constructor(
    private renderer: Renderer2
  ) {}

  public writeValue(value: any): void {
    this.renderer.setProperty(this.textarea.nativeElement, 'textContent', value);
  }

  public registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  public inputChange($event): void {
    this.onChange($event.target.innerText);
  }

  public registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }

  public setDisabledState?(isDisabled: boolean): void {
    const action = isDisabled ? 'addClass' : 'removeClass';
    this.renderer[action](this.textarea.nativeElement, 'tdp-textarea-disabled');
  }

}

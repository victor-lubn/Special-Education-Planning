import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../app.module';

import { SelectComponent } from './select.component';

describe('SelectComponent', () => {
  let component: SelectComponent;
  let fixture: ComponentFixture<SelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SelectComponent],
      imports: [TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: (createTranslateLoader),
          deps: [HttpClient]
        }
      }), HttpClientModule]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should the panel be opened when select if pressed', () => {
    const selectElement = fixture.nativeElement.querySelector('.tdp-atoms-select-container') as HTMLDivElement
    selectElement.dispatchEvent(new Event('click'));
    fixture.detectChanges();
    const panel = fixture.nativeElement.querySelector('.tdp-atoms-select-panel') as HTMLDivElement
    expect(panel).toBeTruthy()
  })
  it('should be able to select the value from list in the panel', () => {
    const options = [
      { value: 'steak-0', text: 'Steak' },
      { value: 'pizza-1', text: 'Pizza' },
      { value: 'tacos-2', text: 'Tacos' },
    ]
    component.options = options
    const inputElement = fixture.nativeElement.querySelector('.tdp-atoms-select-container-input') as HTMLInputElement
    inputElement.dispatchEvent(new Event('focus'));
    fixture.detectChanges();
    component.selectOption(options[0])
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(inputElement.value).toBe('Steak')
    })
    fixture.detectChanges()
    const panel = fixture.nativeElement.querySelector('.tdp-atoms-select-panel') as HTMLDivElement
    expect(panel).toBeFalsy()
  })
});

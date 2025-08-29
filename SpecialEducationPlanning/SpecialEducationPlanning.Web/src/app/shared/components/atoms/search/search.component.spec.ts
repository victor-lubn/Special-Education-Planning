import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ButtonComponent } from '../button/button.component';
import { IconComponent } from '../icon/icon.component';
import { InputComponent } from '../input/input.component';

import { SearchComponent } from './search.component';

describe('SearchComponent', () => {
  let component: SearchComponent;
  let fixture: ComponentFixture<SearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, FormsModule, TranslateModule.forRoot()],
      declarations: [SearchComponent, InputComponent, ButtonComponent, IconComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.nativeElement.remove()
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should enter the "New value" to the input field', () => {
    const searchBar = fixture.nativeElement.querySelector('.form-field input').value = "New value"

    expect(searchBar).toContain('New value')
  })
  it('should delete the "New value" in the input field', () => {
    const searchBar = fixture.nativeElement.querySelector('.form-field input')
    searchBar.value = "New value"

    component.clearAllForms();

    expect(searchBar.value).toBe('')
  })
  it('should open the advanced search panel', () => {

    fixture.detectChanges()
    component.toggleAdvancedSearch()
    fixture.detectChanges()
    const advancedSearchPanel = fixture.nativeElement.querySelector('.tdp-search-advanced-search')

    expect(advancedSearchPanel).toBeTruthy()
  })
  it('should open the advanced search panel', () => {
    component.toggleAdvancedSearch()
    fixture.detectChanges()
    const advancedSearchPanel = fixture.nativeElement.querySelector('.tdp-search-advanced-search')

    expect(advancedSearchPanel).toBeTruthy()
  })
  it('should add value from ProjectID field', () => {
    component.toggleAdvancedSearch()
    fixture.detectChanges()
    const advancedSearchPanel = fixture.nativeElement.querySelector('.tdp-search-advanced-search')
    const tdpInput = advancedSearchPanel.querySelector('.tdp-input')
    tdpInput.value = 'New value'

    expect(tdpInput.value).toBe('New value')
  })
  it('should add delete value from ProjectID field', () => {
    component.toggleAdvancedSearch()
    fixture.detectChanges()
    const advancedSearchPanel = fixture.nativeElement.querySelector('.tdp-search-advanced-search')
    const tdpInput = advancedSearchPanel.querySelector('.tdp-input')
    tdpInput.value = 'New value'
    component.clearAllForms();
    fixture.detectChanges()

    fixture.whenStable().then(() => {
      expect(tdpInput.value).toBe('')
    })
  })

});

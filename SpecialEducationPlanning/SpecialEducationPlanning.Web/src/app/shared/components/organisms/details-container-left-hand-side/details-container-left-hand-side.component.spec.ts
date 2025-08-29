import { CommonModule } from "@angular/common";
import { ChangeDetectorRef } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { TranslateModule } from "@ngx-translate/core";
import { DetailsContainerLeftHandSideComponent } from "./details-container-left-hand-side.component";


describe('DetailsContainerLeftHandSideComponent', () => {
  let component: DetailsContainerLeftHandSideComponent;
  let fixture: ComponentFixture<DetailsContainerLeftHandSideComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [DetailsContainerLeftHandSideComponent],
      imports: [
          CommonModule,
          TranslateModule.forRoot(),
      ],
      providers: [
          ChangeDetectorRef
      ]
    });

    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailsContainerLeftHandSideComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
      expect(component.selectedTab).toBe('plan');
      expect(component.endUserDisabled).toBe(false);
  });

  it('should click on first tab', () => {
      spyOn(component, 'onSelectTab');
      const button = fixture.debugElement.nativeElement.querySelectorAll('.details-tab')[0];
      button.click();
      fixture.detectChanges();
      expect(component.onSelectTab).toHaveBeenCalled();
  });

  it('should click on second tab', () => {
    spyOn(component, 'onSelectTab');
    const button = fixture.debugElement.nativeElement.querySelectorAll('.details-tab')[1];
    button.click();
    fixture.detectChanges();
    expect(component.onSelectTab).toHaveBeenCalled();
  });
});

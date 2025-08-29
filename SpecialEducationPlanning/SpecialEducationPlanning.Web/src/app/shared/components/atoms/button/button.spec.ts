import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ButtonComponent } from './button.component';

@Component({
    template: `<tdp-button [size]="size" [color]="color" [tooltip]="tooltip" [icon]="icon"><div tooltip>{{tooltip}}</div><i class="fas fa-address-card" icon-button></i><div title>Test button</div></tdp-button>`
})
class TestComponent extends ButtonComponent {


    handleOnClick() {
    }
}

describe('Button Component', () => {
    let component: ButtonComponent;
    let fixture: ComponentFixture<TestComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ButtonComponent, TestComponent]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(TestComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        fixture.nativeElement.remove()
    });

    it('should component be created with navy color and small size property', () => {
        component.size = 'small'
        component.color = "navy";
        fixture.detectChanges();
        const buttonElement: HTMLButtonElement = fixture.nativeElement.querySelector('button')!;
        fixture.detectChanges();
        expect(buttonElement).toHaveClass('tdp-atom-button-navy');
        expect(buttonElement).toHaveClass('tdp-atom-button-small');
    });

    it('should component be created with white color, small size property and with tooltop content "New tooltip"', () => {
        component.tooltip = "New tooltip"
        fixture.detectChanges();
        const buttonElement: HTMLButtonElement = fixture.nativeElement.querySelector('button')!;
        const tooltip: HTMLDivElement = fixture.nativeElement.querySelector('.tdp-atom-button-tooltip > div')!;
        fixture.detectChanges();
        expect(buttonElement).toHaveClass('tdp-atom-button-white');
        expect(buttonElement).toHaveClass('tdp-atom-button-small');
        expect(tooltip.textContent).toBe('New tooltip');
    });

    it('should component be created with green color, large size and an fontawesome icon class "fas fa-address-card"', () => {
        component.color = "green"
        component.size = "large"
        component.icon = true
        fixture.detectChanges();
        const buttonElement: HTMLButtonElement = fixture.nativeElement.querySelector('button')!;
        const icon: HTMLElement = fixture.nativeElement.querySelector('.fas.fa-address-card')!;
        fixture.detectChanges();
        expect(buttonElement).toHaveClass('tdp-atom-button-green');
        expect(buttonElement).toHaveClass('tdp-atom-button-large');
        expect(icon).toHaveClass('fa-address-card')
    });
});

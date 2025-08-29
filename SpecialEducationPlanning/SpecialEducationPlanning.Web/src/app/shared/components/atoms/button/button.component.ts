import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'tdp-button',
  templateUrl: 'button.component.html',
  styleUrls: ['button.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ButtonComponent implements OnInit {

  @Input() type?: string = 'button';
  @Input() form?: string;
  @Input() size?: "small" | "medium" | "large" | "xl" = "small";
  @Input() color?: "navy" | "white" | "green" | "ghost" | "text-only" = "white";
  @Input() icon?: boolean = false;
  @Input() iconRight?: boolean = false;
  @Input() whiteBlueButton?: boolean = false;
  @Input() whiteGreenButton?: boolean = false;
  @Input() tooltip?: string;
  @Input() disabled?: boolean = false;
  activeTooltip?: boolean = false;

  @Output() onClick?= new EventEmitter<Event>();

  constructor() { }

  ngOnInit(): void { }

  handleOnClick() {
    this.onClick.emit();
  }

  showTooltip() {
    this.activeTooltip = true
  }

  hideTooltip() {
    this.activeTooltip = false
  }
}

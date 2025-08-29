import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'tdp-create-button',
  templateUrl: './create-button.component.html',
  styleUrls: ['./create-button.component.scss']
})
export class CreateButtonComponent implements OnInit {

  @Input() 
  isEnabled?: boolean = true;
  
  @Input() 
  text?: string = undefined;

  @Output() 
  onClick = new EventEmitter<void>();

  constructor() { }

  ngOnInit(): void {
  }

  handleOnClick() {
    this.onClick.emit();
  }
}

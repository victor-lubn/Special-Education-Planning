import { Location } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'tdp-customer-header',
  templateUrl: './customer-header.component.html',
  styleUrls: ['./customer-header.component.scss']
})
export class CustomerHeaderComponent {
  @Input() builderName?: string;
  @Input() creditAccount?: string;
  @Output() goBack = new EventEmitter<void>();

  constructor(private location: Location) {

  }

  handleGoBack() {
    this.location.back()
  }
}

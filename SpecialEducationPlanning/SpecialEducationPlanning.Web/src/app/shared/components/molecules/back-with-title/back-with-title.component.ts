import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'tdp-back-with-title',
  templateUrl: './back-with-title.component.html',
  styleUrls: ['./back-with-title.component.scss']
})
export class BackWithTitleComponent implements OnInit {

  @Input()
  title: string = '';

  @Output()
  onClick = new EventEmitter<void>();

  constructor() { }

  ngOnInit(): void {
  }

  onBack(): void {
    this.onClick.emit();
  }

}

import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'tdp-top-banner',
  templateUrl: './top-banner.component.html',
  styleUrls: ['./top-banner.component.scss']
})
export class TopBannerComponent implements OnInit {
  @Input() informative?: boolean
  @Input() error?: boolean
  @Input() offline?: boolean

  constructor() { }

  ngOnInit(): void {
  }

}

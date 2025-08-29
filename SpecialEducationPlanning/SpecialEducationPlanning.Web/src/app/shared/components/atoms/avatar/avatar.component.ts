import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'tdp-avatar',
  templateUrl: './avatar.component.html',
  styleUrls: ['./avatar.component.scss']
})
export class AvatarComponent {

  @Input()
  public initials: string;

  @Input()
  public sizeAvatar: 'small' | 'large';

  @Output()
  public onClick = new EventEmitter<Event>();

  constructor() { }

  handleOnClick() {
    this.onClick.emit();
  }

}

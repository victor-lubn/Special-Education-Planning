import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AvatarComponent } from '../../atoms/avatar/avatar.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { TranslateModule } from "@ngx-translate/core";

import { MenuDetailsComponent } from './menu-details.component'

describe('MenuDetailsComponent', () => {
  let component: MenuDetailsComponent;
  let fixture: ComponentFixture<MenuDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MenuDetailsComponent, IconComponent, AvatarComponent, ButtonComponent ],
      imports: [ TranslateModule.forRoot()]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MenuDetailsComponent);
    component = fixture.componentInstance;
    let userInfo = { 
      username: 'DZ99TDP Educationer',
      email: 'DZ99TDP.Educationer@hwdn.co.uk',
      role: 'Educationer',
      Aiep: 'DF31',
      initials: 'DD'};
    component.userInfo = userInfo;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should change the userInfo', () => {
    let userInfoChanged = { 
      username: 'Marian Smith',
      email: 'marian.Educationer@hwdn.co.uk',
      role: 'Educationer',
      Aiep: 'DF31',
      initials: 'MS'};
    component.userInfo = userInfoChanged;
    fixture.detectChanges();
    expect(component.userInfo).toBe(userInfoChanged);
  })
});



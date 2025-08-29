import { HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from "@ngx-translate/core";
import { BlockUIService } from '../../../../core/block-ui/block-ui.service';
import { ElectronService } from '../../../../core/electron-api/electron.service';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { NetworkStatusService } from '../../../../core/services/network-status/network-status.service';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { MenuComponent } from './menu.component';


describe('MenuComponent', () => {
  let component: MenuComponent;
  let fixture: ComponentFixture<MenuComponent>;
  let debugElement;
  let element;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [MenuComponent],
      imports: [TranslateModule.forRoot(), HttpClientModule, MatDialogModule, RouterTestingModule],
      providers: [NetworkStatusService, BlockUIService, ErrorLogService, NetworkStatusService, ElectronService]
    })
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
  });



  beforeEach(() => {
    fixture = TestBed.createComponent(MenuComponent);
    component = fixture.componentInstance;
    let userInfo = {
      username: 'DZ99TDP Educationer',
      email: 'DZ99TDP.Educationer@hwdn.co.uk',
      role: 'Educationer',
      Aiep: 'DF31',
      initials: 'DD'
    };
    component.userInformation = userInfo;
    debugElement = fixture.debugElement;
    element = debugElement.nativeElement;
    fixture.detectChanges();
  });

  afterEach(() => {
    document.body.removeChild(element);
  })

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should update the UserInformation()', () => {
    let userInfoChanged = {
      username: 'Marian Smith',
      email: 'marian.Educationer@hwdn.co.uk',
      role: 'Educationer',
      Aiep: 'DF31',
      initials: 'MS'
    };
    component.userInformation = userInfoChanged;
    fixture.detectChanges();
    expect(component.userInformation).toBe(userInfoChanged);
  })

});



import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { UserInfoService } from 'src/app/core/services/user-info/user-info.service';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { AuthService } from '../../core/auth/auth.service';
import { NetworkStatusService } from '../../core/services/network-status/network-status.service';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { AuthenticationResult, EventMessage, EventType, InteractionStatus } from '@azure/msal-browser';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'tdp-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent extends BaseComponent implements OnInit {

  public isCallback = false;
  public loadingUserInfo: boolean;
  checkingNetwork: boolean = false

  constructor(    
    private _auth: AuthService,
    private userInfo: UserInfoService,
    private dialogs: DialogsService,
    private route: ActivatedRoute,
    private networkStatus: NetworkStatusService,
    private authService: MsalService, 
    private msalBroadcastService: MsalBroadcastService
  ) {
    super();
    this.loadingUserInfo = false;
  }

  ngOnInit() {
    this.msalBroadcastService.msalSubject$
      .pipe(
        filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS),
      )
      .subscribe((result: EventMessage) => {        
        const payload = result.payload as AuthenticationResult;
        this.authService.instance.setActiveAccount(payload.account);
      });
    
    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None)
      )
      .subscribe(() => {        
        this.callback();
      })

    const getNetworkSubscription = this.networkStatus.getApiConnection().subscribe(() => {
      this.checkingNetwork = true
      this.route.data.subscribe((data: Data) => {
        if (data['isCallback']) {          
          this.isCallback = data['isCallback'];
          this.callback();
        }
      })
    }, error => {
      this.checkingNetwork = false
      this.navigateTo('/loginOffline');
    })
    this.entitySubscriptions.push(getNetworkSubscription)
  }

  public login(): void {
    this._auth.login();
  }

  callback(): void {
    this.loadingUserInfo = true;
    this.userInfo.loadUserInfo().then((success) => {
      this.navigateTo('/home');
    },
      (error) => {
        if (error.status === 403) {
          this.isCallback = false;
          this.dialogs.simpleInformation(
            'dialog.accessError',
            'dialog.accessErrorText'
          )
            .then(() => {
              this._auth.logout();
            });
        }
      });
  }

}

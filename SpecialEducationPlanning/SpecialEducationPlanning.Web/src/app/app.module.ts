import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { IPublicClientApplication, LogLevel, PublicClientApplication, } from '@azure/msal-browser';
import {
  MSAL_INSTANCE,
  MsalBroadcastService,
  MsalModule,
  MsalRedirectComponent,
  MsalService,
} from '@azure/msal-angular';

import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { AppRoutingModule } from './app.routing';
import { MiddlewareModule } from './middleware/middleware.module';
import { environment } from '../environments/environment';
import { LoginOfflineComponent } from './pages/offline/login-offline/login-offline.component';
import { MatNativeDateModule } from '@angular/material/core';

export { environment };

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, "./assets/i18n/", ".json");
}
const _auth = environment.AuthSettings;

export function getMsalConfig() {
  return _auth || environment.AuthSettings;
}

const ConfiguredTranslateModule = TranslateModule.forRoot({
  loader: {
    provide: TranslateLoader,
    useFactory: createTranslateLoader,
    deps: [HttpClient],
  },
});

export function loggerCallback(logLevel: LogLevel, message: string) {
  console.log(message);
}

export function MSALInstanceFactory(): IPublicClientApplication {
  const conf = {
    ...(_auth || environment.AuthSettings),
    system: {
      loggerOptions: {
        loggerCallback,
        logLevel: LogLevel.Info,
        piiLoggingEnabled: false,
      },
    },
  };
  return new PublicClientApplication(conf);
}

@NgModule({
  declarations: [AppComponent, LoginOfflineComponent],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    CoreModule,
    MiddlewareModule,
    MsalModule,
    ConfiguredTranslateModule,
    MatNativeDateModule,
  ],
  providers: [
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
    },
    // TODO remove if it will not being used! (at DV starting read all missed events sent from 3DC and send it to the backend)
    // {
    //   provide: APP_INITIALIZER,
    //   useFactory: (planService: PlanService, middlewareService: MiddlewareService) => {
    //     return () => {
    //       const missedEvents = middlewareService.get3DCEventsFromFiles();
    //       console.log(missedEvents)
    //       // for (const event of missedEvents) {
    //       //   console.log(event);
    //       //   // planService.newPlanVersion({
    //       //   //   EducationTool3DCVersionId: event.EducationTool3DCVersionId,
    //       //   //   LastKnown3DCVersion: event.LastKnown3DCVersion,
    //       //   //   EducationTool3DCPlanId: event.EducationTool3DCPlanId,
    //       //   //   PlanId: event.PlanId,
    //       //   //   VersionNumber: event.VersionNumber,
    //       //   //   CatalogId: event.CatalogId
    //       //   // })
    //       // }
    //     };
    //   },
    //   deps: [PlanService, MiddlewareService],
    //   multi: true
    // },
    MsalService,
    MsalBroadcastService,
  ],
  bootstrap: [AppComponent, MsalRedirectComponent],
})
export class AppModule {}


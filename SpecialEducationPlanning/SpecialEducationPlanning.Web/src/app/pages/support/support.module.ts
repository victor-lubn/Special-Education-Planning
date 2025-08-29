import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { SupportRoutingModule } from './support.routing';
import { SupportDashboardComponent } from './support-dashboard/support-dashboard.component';
import { TemplatesModule } from '../../shared/components/templates/templates.module';

@NgModule({
  imports: [
    SharedModule,
    SupportRoutingModule,
    TemplatesModule
  ],
  declarations: [
    SupportDashboardComponent
  ]
})
export class SupportModule { }

import { NgModule } from '@angular/core';
import { TemplatesModule } from '../../../shared/components/templates/templates.module';

import { SharedModule } from '../../../shared/shared.module';
import { ActionsLogsListComponent } from './actions-logs-list/actions-logs-list.component';
import { ActionsLogsRoutingModule } from './actions-logs.routing';


@NgModule({
  imports: [
    SharedModule,    
    ActionsLogsRoutingModule,
    TemplatesModule
  ],
  declarations: [
    ActionsLogsListComponent
  ]
})
export class ActionsLogsModule {}

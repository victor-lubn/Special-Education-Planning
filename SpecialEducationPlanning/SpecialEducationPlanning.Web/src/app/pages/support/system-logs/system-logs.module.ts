import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { SystemLogsRoutingModule } from './system-logs.routing';
import { SystemLogsListComponent } from './system-logs-list/system-logs-list.component';
import { TemplatesModule } from '../../../shared/components/templates/templates.module';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  imports: [
    SharedModule,
    SystemLogsRoutingModule,
    TemplatesModule,
    MatTooltipModule
  ],
  declarations: [
    SystemLogsListComponent
  ]
})
export class SystemLogsModule { }

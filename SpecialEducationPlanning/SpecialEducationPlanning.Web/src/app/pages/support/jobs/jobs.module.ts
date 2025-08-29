import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { JobsRoutingModule } from './jobs.routing';
import { JobsComponent } from './jobs.component';

@NgModule({
  imports: [
    SharedModule,
    JobsRoutingModule
  ],
  declarations: [
    JobsComponent
  ]
})
export class JobsModule {}

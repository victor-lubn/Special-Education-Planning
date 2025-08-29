import { NgModule } from '@angular/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TemplatesModule } from 'src/app/shared/components/templates/templates.module';

import { SharedModule } from '../../../shared/shared.module';
import { ReleaseNotesListComponent } from './release-notes-list/release-notes-list.component';
import { ReleaseNotesRoutingModule } from './release-notes.routing';

@NgModule({
  imports: [
    SharedModule,
    ReleaseNotesRoutingModule,
    TemplatesModule,
    MatTooltipModule
  ],
  declarations: [
    ReleaseNotesListComponent
  ]
})
export class ReleaseNotesModule {}

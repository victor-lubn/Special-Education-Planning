import { NgModule } from '@angular/core';

import { SharedModule } from '../../../../shared/shared.module';
import { RolesRoutingModule } from './roles.routing';
import { RolesListComponent } from './roles-list/roles-list.component';
import { TemplatesModule } from '../../../../shared/components/templates/templates.module';

@NgModule({
  imports: [
    SharedModule,
    RolesRoutingModule,
    TemplatesModule
  ],
  declarations: [
    RolesListComponent
  ]
})
export class RolesModule { }

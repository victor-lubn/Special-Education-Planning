import { NgModule } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';

import { SharedModule } from '../../../shared/shared.module';
import { AiepsRoutingModule } from './Aieps.routing';
import { AiepsListComponent } from './lists/Aieps-list/Aieps-list.component';
import { AiepCreateComponent } from './Aiep-create/Aiep-create.component';
import { AiepDetailsComponent } from './Aiep-details/Aiep-details.component';
import { CountriesListComponent } from './lists/countries-list/countries-list.component';
import { RegionsListComponent } from './lists/regions-list/regions-list.component';
import { AreasListComponent } from './lists/areas-list/areas-list.component';
import { AiepGeneralComponent } from './Aiep-general/Aiep-general';
import { TemplatesModule } from 'src/app/shared/components/templates/templates.module';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  imports: [
    SharedModule,
    AiepsRoutingModule,
    MatExpansionModule,
    TemplatesModule,
    MatTooltipModule
  ],
  declarations: [
    AiepsListComponent,
    AiepCreateComponent,
    AiepDetailsComponent,
    CountriesListComponent,
    RegionsListComponent,
    AreasListComponent,
    AiepGeneralComponent
  ]
})
export class AiepsModule {}


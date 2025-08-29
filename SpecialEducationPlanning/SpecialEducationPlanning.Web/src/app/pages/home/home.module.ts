import { NgModule } from '@angular/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';

import { SharedModule } from '../../shared/shared.module';
import { HomeRoutingModule } from './home.routing';
import { HomeComponent } from './home.component';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';
import { TemplatesModule } from 'src/app/shared/components/templates/templates.module';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  imports: [
    SharedModule,
    MatTabsModule,
    HomeRoutingModule,
    MatButtonModule,
    MatIconModule,
    MatAutocompleteModule,
    MatCheckboxModule,
    TemplatesModule,
    MatTooltipModule
  ],
  declarations: [
    HomeComponent
  ],
  providers: [{ provide: MAT_TAB_GROUP, useValue: {} }]
})
export class HomeModule {}

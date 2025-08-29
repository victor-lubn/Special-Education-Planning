import { NgModule } from '@angular/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SidebarModule } from '../../shared/components/sidebar/sidebar.module';
import { SharedModule } from '../../shared/shared.module';
import { PlanCreateComponent } from './plan-create/plan-create.component';
import { PlanDetailsComponent } from './plan-details/plan-details.component';
import { PlanRoutingModule } from './plan.routing';
@NgModule({
  imports: [
    SharedModule,
    PlanRoutingModule,
    // Angular Material modules
    MatIconModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatExpansionModule,
    MatTooltipModule,
    MatTabsModule,
    SidebarModule
  ],
  declarations: [
    PlanDetailsComponent,
    PlanCreateComponent,
  ],
  providers: [
    { provide: MAT_DIALOG_DATA, useValue: {} },
    { provide: MAT_TAB_GROUP, useValue: {} }
  ]
})
export class PlanModule { }

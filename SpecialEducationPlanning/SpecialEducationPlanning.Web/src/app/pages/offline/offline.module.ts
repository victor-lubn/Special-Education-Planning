import { NgModule } from '@angular/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';

import { SharedModule } from '../../shared/shared.module';
import { OfflineRoutingModule } from './offline.routing';
import { PlanListOfflineComponent } from './plan-offline/plan-list-offline/plan-list-offline.component';
import { PlanCreateOfflineComponent } from './plan-offline/plan-create-offline/plan-create-offline.component';
import { PlanDetailsOfflineComponent } from './plan-offline/plan-details-offline/plan-details-offline.component';
import { LayoutOfflineComponent } from './layout-offline/layout-offline.component';
import { TopbarOfflineComponent } from './layout-offline/topbar-offline/topbar-offline.component';
import { BlockUIService } from '../../core/block-ui/block-ui.service';
import { BlockUIModule } from 'ng-block-ui';
import { MatTabsModule } from '@angular/material/tabs';
import { TopbarMenuModule } from '../../shared/components/molecules/topbar-menu/topbar-menu.module';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  imports: [
    SharedModule,
    MatExpansionModule,
    OfflineRoutingModule,
    BlockUIModule.forRoot(),
    MatMenuModule,
    MatDividerModule,
    MatSidenavModule,
    TopbarMenuModule,
    MatTabsModule,
    MatTooltipModule,
  ],
  declarations: [
    LayoutOfflineComponent,
    TopbarOfflineComponent,
    PlanListOfflineComponent,
    PlanCreateOfflineComponent,
    PlanDetailsOfflineComponent
  ],
  providers: [BlockUIService]
})
export class OfflineModule { }

import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';

import { SharedModule } from '../../shared/shared.module';
import { BuilderRoutingModule } from './builder.routing';
import { BuilderGeneralComponent } from './builder-general/builder-general.component';
import { BuilderDetailsComponent } from './builder-details/builder-details.component';
import { BuilderCreateComponent } from './builder-create/builder-create.component';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
    imports: [
        SharedModule,
        BuilderRoutingModule,
        // Angular Material modules
        MatAutocompleteModule,
        MatIconModule,
        MatButtonModule,
        MatExpansionModule,
        MatCheckboxModule,
        MatTabsModule,
        MatTooltipModule
    ],
    declarations: [
        BuilderGeneralComponent,
        BuilderDetailsComponent,
        BuilderCreateComponent
    ],
    providers: [
        { provide: MAT_DIALOG_DATA, useValue: {} },
        { provide: MAT_TAB_GROUP, useValue: {} }
    ]
})
export class BuilderModule {}

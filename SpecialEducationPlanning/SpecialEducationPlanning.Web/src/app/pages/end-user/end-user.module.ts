import { NgModule } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';

import { SharedModule } from '../../shared/shared.module';

import { EndUserCreateComponent } from './end-user-create/end-user-create.component';
import { EndUserDetailsComponent } from './end-user-details/end-user-details.component';
import { EndUserHomeComponent } from './end-user-home/end-user-home.component';

@NgModule({   
imports: [ 
    SharedModule,    
    MatExpansionModule
],
declarations: [
    EndUserCreateComponent,
    EndUserDetailsComponent,
    EndUserHomeComponent
]
})

export class EndUserModule {}

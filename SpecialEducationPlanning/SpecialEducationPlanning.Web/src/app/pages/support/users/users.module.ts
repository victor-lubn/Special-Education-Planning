import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { UsersRoutingModule } from './users.routing';
import { UserListComponent } from './user-list/user-list.component';
import { NewUserComponent } from './new-user/new-user.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { UserDetailsComponent } from './user-details/user-details.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TemplatesModule } from '../../../shared/components/templates/templates.module';

@NgModule({
    imports: [
        SharedModule,
        UsersRoutingModule,
        MatExpansionModule,
        MatTooltipModule,
        TemplatesModule
    ],
  declarations: [
    UserListComponent,
    NewUserComponent,
    UserDetailsComponent
  ]
})
export class UsersModule {}

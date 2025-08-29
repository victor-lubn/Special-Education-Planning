import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { TopbarMenuComponent } from './topbar-menu.component';
import { TopbarActionsComponent } from './topbar-actions/topbar-actions.component';
import { TopbarTitleComponent } from './topbar-title/topbar-title.component';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
      TopbarMenuComponent,
      TopbarActionsComponent,
      TopbarTitleComponent
    ],
    exports: [
      TopbarMenuComponent,
      TopbarActionsComponent,
      TopbarTitleComponent
    ]
})
export class TopbarMenuModule
{
}
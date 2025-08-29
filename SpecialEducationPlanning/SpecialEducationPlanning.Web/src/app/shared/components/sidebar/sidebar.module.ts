import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ResizableDirective } from './resizable.directive';

import { SidebarComponent } from './sidebar.component';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        SidebarComponent,
        ResizableDirective
    ],
    exports     : [
        SidebarComponent,
        ResizableDirective
    ]
})
export class SidebarModule
{
}

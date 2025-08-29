import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { MainViewComponent } from "./main-view/main-view.component";
import { AiepDetailsTemplateComponent } from './Aiep-details-template/Aiep-details-template.component';
import { AiepCreateNewTemplateComponent } from "./Aiep-create-new-template/Aiep-create-new-template.component";

@NgModule({
  imports: [
    CommonModule,
  ],
  declarations: [
    MainViewComponent,
    AiepDetailsTemplateComponent,
    AiepCreateNewTemplateComponent
  ],
  exports     : [
    MainViewComponent,
    AiepDetailsTemplateComponent,
    AiepCreateNewTemplateComponent
  ]
})
export class TemplatesModule
{
}


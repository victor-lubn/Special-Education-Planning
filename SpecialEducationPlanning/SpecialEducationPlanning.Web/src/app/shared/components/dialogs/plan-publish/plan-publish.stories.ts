import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { action } from '@storybook/addon-actions';
import { PlanPublishComponent } from './plan-publish.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { ModalComponent } from '../../atoms/modal/modal.component';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { InputComponent } from '../../atoms/input/input.component'
import { ReactiveFormsModule } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ApiService } from '../../../../core/api/api.service';
import { PlanService } from '../../../../core/api/plan/plan.service';
import { TranslateService } from '@ngx-translate/core';
import { ActionLogsService } from '../../../../core/api/action-logs/action-logs.service';
import { AreaService } from '../../../../core/api/area/area.service';
import { BuilderService } from '../../../../core/api/builder/builder.service';
import { CountryService } from '../../../../core/api/country/country.service';
import { CsvService } from '../../../../core/api/csv/csv.service';
import { AiepService } from '../../../../core/api/Aiep/Aiep.service';
import { OmniSearchService } from '../../../../core/api/omni-search/omni-search.service';
import { PostcodeService } from '../../../../core/api/postcode/postcode.service';
import { RegionService } from '../../../../core/api/region/region.service';
import { ReleaseInfoService } from '../../../../core/api/release-info/release-info.service';
import { RoleService } from '../../../../core/api/role/role.service';
import { SortingFilteringService } from '../../../../core/api/sorting-filtering/sorting-filtering.service';
import { SystemLogService } from '../../../../core/api/system-log/system-log.service';
import { UserService } from '../../../../core/api/user/user.service';
import { EndUserService } from '../../../../core/services/end-user/end-user.service';
import { NotificationsService } from 'angular2-notifications';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { Injector } from '@angular/core';


export default {
    component: PlanPublishComponent,
    decorators: [
        moduleMetadata({
            declarations: [PlanPublishComponent, ButtonComponent, IconComponent, ModalComponent, TextAreaComponent, InputComponent ],
            imports: [CommonModule, ReactiveFormsModule],
            providers: [
              ApiService,
              ServiceInjector,
              { provide: MatDialog, useValue: {} },
              { provide: DialogsService, useValue: {} },
              { provide: PlanService, useValue: {} },
              { provide: BuilderService, useValue: {} },
              { provide: OmniSearchService, useValue: {} },
              { provide: PostcodeService, useValue: {} },
              { provide: ReleaseInfoService, useValue: {} },
              { provide: AiepService, useValue: {} },
              { provide: CountryService, useValue: {} },
              { provide: SortingFilteringService, useValue: {} },
              { provide: CsvService, useValue: {} },
              { provide: AreaService, useValue: {} },
              { provide: RegionService, useValue: {} },
              { provide: RoleService, useValue: {} },
              { provide: SystemLogService, useValue: {} },
              { provide: ActionLogsService, useValue: {} },
              { provide: EndUserService, useValue: {} },
              { provide: MatDialogRef, useValue: {} },
              { provide: UserService, useValue: {} },
              { provide: NotificationsService, useValue: {}},
              { provide: UserInfoService, useValue: {}},
              { provide: ErrorLogService, useValue: {} },
              { provide: MAT_DIALOG_DATA, useValue: {} },
              { provide: TranslateService, useValue: {} }
            ]
          })
    ],
    argTypes: {
    },
    title: 'Organism/Plan Publish'
} as Meta;

const Template: Story<PlanPublishComponent> = (args) => ({
    props: {
        ...args,
    },
    template: 
    `
    <tdp-plan-publish></tdp-plan-publish>
    `
});

export const PlanPublish = Template.bind({});
PlanPublish.args = {
}

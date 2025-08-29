import { CommonModule } from "@angular/common";
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { createTranslateLoader } from "../../../../app.module";
import { ActionLogsService } from "../../../../core/api/action-logs/action-logs.service";
import { ApiService } from "../../../../core/api/api.service";
import { AreaService } from "../../../../core/api/area/area.service";
import { BuilderService } from "../../../../core/api/builder/builder.service";
import { CountryService } from "../../../../core/api/country/country.service";
import { CsvService } from "../../../../core/api/csv/csv.service";
import { AiepService } from "../../../../core/api/Aiep/Aiep.service";
import { OmniSearchService } from "../../../../core/api/omni-search/omni-search.service";
import { PlanService } from "../../../../core/api/plan/plan.service";
import { PostcodeService } from "../../../../core/api/postcode/postcode.service";
import { RegionService } from "../../../../core/api/region/region.service";
import { ReleaseInfoService } from "../../../../core/api/release-info/release-info.service";
import { RoleService } from "../../../../core/api/role/role.service";
import { SortingFilteringService } from "../../../../core/api/sorting-filtering/sorting-filtering.service";
import { SystemLogService } from "../../../../core/api/system-log/system-log.service";
import { UserService } from "../../../../core/api/user/user.service";
import { EndUserService } from "../../../../core/services/end-user/end-user.service";
import { UserInfoService } from "../../../../core/services/user-info/user-info.service";
import { DetailsContainerLeftHandSideComponent } from "./details-container-left-hand-side.component";

export default {
    component: DetailsContainerLeftHandSideComponent,
    decorators: [
        moduleMetadata({
            declarations: [DetailsContainerLeftHandSideComponent],
            imports: [
                CommonModule,
                TranslateModule.forRoot({
                    loader: {
                        provide: TranslateLoader,
                        useFactory: (createTranslateLoader),
                        deps: [HttpClient]
                    }
                }),
                HttpClientModule
            ],
            providers: [
                ApiService,
                PlanService,
                UserInfoService,
                BuilderService,
                OmniSearchService,
                PostcodeService,
                ReleaseInfoService,
                AiepService,
                CsvService,
                CountryService,
                SortingFilteringService,
                UserService,
                AreaService,
                RegionService,
                RoleService,
                SystemLogService,
                ActionLogsService,
                EndUserService
            ]
        })
    ],
    title: 'Organism/Details Container'
} as Meta;

const Template: Story<DetailsContainerLeftHandSideComponent> = (args) => ({
    props: {
        ...args
    },
    template: `
    <tdp-details-container-left-hand-side>
    </tdp-details-container-left-hand-side>
    `
});

export const DetailsContainer = Template.bind({});

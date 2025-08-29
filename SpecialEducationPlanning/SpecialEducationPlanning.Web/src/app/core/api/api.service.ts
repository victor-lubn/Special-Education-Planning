import { Injectable } from '@angular/core';

import { PlanService } from './plan/plan.service';
import { BuilderService } from './builder/builder.service';
import { OmniSearchService } from './omni-search/omni-search.service';
import { PostcodeService } from './postcode/postcode.service';
import { ReleaseInfoService } from './release-info/release-info.service';
import { AiepService } from './Aiep/Aiep.service';
import { CsvService } from './csv/csv.service';
import { CountryService } from './country/country.service';
import { SortingFilteringService } from './sorting-filtering/sorting-filtering.service';
import { UserService } from './user/user.service';
import { AreaService } from './area/area.service';
import { RegionService } from './region/region.service';
import { RoleService } from './role/role.service';
import { SystemLogService } from './system-log/system-log.service';
import { ActionLogsService } from './action-logs/action-logs.service';
import { EndUserService } from '../services/end-user/end-user.service';
import { PublishSystemService } from './publish-system/publish-system.service';
import { ProjectService } from './project/project.service';

@Injectable()
export class ApiService {

  constructor(
    private planService: PlanService,
    private builderService: BuilderService,
    private omniSearchService: OmniSearchService,
    private postcodeService: PostcodeService,
    private releaseInfoService: ReleaseInfoService,
    private AiepService: AiepService,
    private csvService: CsvService,
    private countryService: CountryService,
    private sortingFilteringService: SortingFilteringService,
    private userService: UserService,
    private areaService: AreaService,
    private regionService: RegionService,
    private roleService: RoleService,
    private systemLogService: SystemLogService,
    private actionLogsService: ActionLogsService,
    private endUserService : EndUserService,
    private publishService: PublishSystemService,
    private projectService: ProjectService
  ) { }

  get plans(): PlanService {
    return this.planService;
  }

  get projects(): ProjectService {
    return this.projectService;
  }

  get builders(): BuilderService {
    return this.builderService;
  }

  get omniSearch(): OmniSearchService {
    return this.omniSearchService;
  }

  get postcodes(): PostcodeService {
    return this.postcodeService;
  }

  get releaseInfo(): ReleaseInfoService {
    return this.releaseInfoService;
  }

  get Aieps(): AiepService {
    return this.AiepService;
  }

  get csv(): CsvService {
    return this.csvService;
  }

  get countries(): CountryService {
    return this.countryService;
  }

  get sortingFiltering(): SortingFilteringService {
    return this.sortingFilteringService;
  }
  get users(): UserService {
    return this.userService;
  }

  get areas(): AreaService {
    return this.areaService;
  }

  get regions(): RegionService {
    return this.regionService;
  }

  get roles(): RoleService {
    return this.roleService;
  }

  get systemLogs(): SystemLogService {
    return this.systemLogService;
  }

  get actionLogs(): ActionLogsService {
    return this.actionLogsService;
  }

  get endUsers() : EndUserService{
    return this.endUserService;
  }

  get publish(): PublishSystemService {
    return this.publishService;
  }

}


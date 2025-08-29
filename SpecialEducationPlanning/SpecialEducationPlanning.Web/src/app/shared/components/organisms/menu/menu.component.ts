import {
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  OnInit,
} from "@angular/core";
import { Router } from "@angular/router";
import { UntypedFormControl } from "@angular/forms";
import { HomeFilterBuilderForm } from "src/app/shared/models/home-filter-builer-form";
import { HomeFilterPlanForm } from "src/app/shared/models/home-filter-plan-form";
import { NetworkStatusService } from "../../../../core/services/network-status/network-status.service";
import { Aiep } from "../../../models/Aiep.model";
import { UserInformation } from "../../../models/user-information";
import { SearchComponent } from "../../atoms/search/search.component";
import { SelectOptionInterface } from "../../atoms/select/select.component";
import { TopbarMenuComponent } from "../../molecules/topbar-menu/topbar-menu.component";
import { CommunicationService } from "src/app/core/services/communication/communication.service";
import { HomeFilterProjectForm } from "src/app/shared/models/home-filter-project-form";

@Component({
  selector: "tdp-menu",
  templateUrl: "./menu.component.html",
  styleUrls: ["./menu.component.scss"],
})
export class MenuComponent implements OnInit {
  public sidebar: string;
  public isOnline: boolean = true;

  @Input()
  supportSelectedAiep: UntypedFormControl;

  @ViewChild(TopbarMenuComponent)
  public topbarMenuComponent: TopbarMenuComponent;

  @ViewChild("searchForm", { static: true })
  public searchForm: SearchComponent;

  @Input()
  public userInformation: UserInformation;

  @Input()
  filteredAiepOptions: Aiep[];

  @Input()
  public showSupport: boolean;

  @Input()
  public showProduction: boolean;

  @Input()
  public activeEnvironment?: string;

  @Input()
  public EducationersOptionsList: SelectOptionInterface[] = [];

  @Output()
  public onHomePage = new EventEmitter<void>();

  @Output()
  public onReleaseNotesDocument = new EventEmitter<void>();

  @Output()
  public onReleaseNotesWeb = new EventEmitter<void>();

  @Output()
  public onAbout = new EventEmitter<void>();

  @Output()
  public onSupportDashboard = new EventEmitter<void>();

  @Output()
  public onShowProjects = new EventEmitter<void>();

  @Output()
  public onLogout = new EventEmitter<void>();

  @Output()
  public onAddSearchTerm = new EventEmitter<string>();

  @Output()
  onAddAdvancedSearchPlans = new EventEmitter<HomeFilterPlanForm>();

  @Output()
  onAddAdvancedSearchProject = new EventEmitter<HomeFilterProjectForm>();

  @Output()
  onAddAdvancedSearchTradeCustomer = new EventEmitter<HomeFilterBuilderForm>();

  @Output()
  onResetFilters = new EventEmitter<void>();

  @Output()
  onSelectedAiepOption = new EventEmitter<any>();

  constructor(
    protected networkService: NetworkStatusService,
    private communication: CommunicationService,
    private router: Router
  ) {
    this.sidebar = "rightSidebar";
  }

  ngOnInit(): void {
    this.networkService.getOnlineStatusSubscription().subscribe((status) => {
      this.isOnline = status;
    });
  }

  public openMenu() {
    this.topbarMenuComponent.openMenuDetails(this.sidebar);
  }

  public handleOpenHomePage() {
    this.router.navigate(['/home']); //TODO
    //this.onHomePage.emit();
  }

  public handleReleaseNotesDocument() {
    this.onReleaseNotesDocument.emit();
  }

  public handleReleaseNotesWeb() {
    this.onReleaseNotesWeb.emit();
  }

  public handleSupportDashboard() {
    this.onSupportDashboard.emit();
  }

  public handleOpenAbout() {
    this.onAbout.emit();
  }

  public handleShowProjects() {
    this.router.navigate(['/project']);
  }

  public handlelogoutUser() {
    this.onLogout.emit();
  }

  public addSearchTermHandler(value: string): void {
    this.onAddSearchTerm.emit(value);
  }

  addAdvancedSearchPlansHandler(value: HomeFilterPlanForm): void {
    this.onAddAdvancedSearchPlans.emit(value);
  }

  addAdvancedSearchProjectsHandler(value: HomeFilterProjectForm): void {
    this.onAddAdvancedSearchProject.emit(value);
  }

  addAdvancedSearchTradeCustomerHandler(value: HomeFilterBuilderForm): void {
    this.onAddAdvancedSearchTradeCustomer.emit(value);
  }

  resetFiltersHandler(): void {
    this.onResetFilters.emit();
  }

  selectedAiepOption(value) {
    this.onSelectedAiepOption.emit(value);
  }
}



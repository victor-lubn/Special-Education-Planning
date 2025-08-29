import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';

import { TranslateModule } from '@ngx-translate/core';
import { PdfViewerModule } from 'ng2-pdf-viewer';

import { PlatformElementDirective } from './directives/platform.directive';
import { PermissionDirective } from './directives/permission.directive';
import { PlanPreviewComponent } from './components/dialogs/plan-preview/plan-preview.component';
import { PlanPublishComponent } from './components/dialogs/plan-publish/plan-publish.component';
import { BuilderSearchResultComponent } from './components/item-list/builder-search-result/builder-search-result.component';
import { PlanSearchResultComponent } from './components/item-list/plan-search-result/plan-search-result.component';
import { ConfirmationDialogComponent } from './components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { SimpleArchiveDialogComponent } from './components/dialogs/simple-archive-dialog/simple-archive-dialog.component';
import { PdfViewerDialogComponent } from './components/dialogs/pdf-viewer-dialog/pdf-viewer-dialog.component';
import { AssignPermissionsDialogComponent } from './components/dialogs/assign-permissions-dialog/assign-permissions-dialog.component';
import { AssignEntitiesComponent } from './components/assign-entities/assign-entities.component';
import { ExistingBuilderDialogComponent } from './components/dialogs/existing-builder-dialog/existing-builder-dialog.component';
import { InformationDialogComponent } from './components/dialogs/information-dialog/information-dialog.component';
import { UnassignedPlanDialogComponent } from './components/dialogs/unassigned-plan-dialog/unassigned-plan-dialog.component';
import { UploadPlanDialogComponent } from './components/dialogs/upload-plan-dialog/upload-plan-dialog.component';
import { DndZoneDirective } from './directives/drag-and-drop-zone.directive';
import { TimelineCommentComponent } from './components/molecules/timeline-comment/timeline-comment.component';
import { TextareaComponent } from './components/textarea/textarea.component';
import { PostcodeComponent } from './components/postcode/postcode.component';
import { PipeModule } from './pipes/pipes.module';
import { UploadReleaseNotesDialogComponent } from './components/dialogs/upload-release-notes-dialog/upload-release-notes-dialog.component';
import { UploadCSVDialogComponent } from './components/dialogs/upload-csv-dialog/upload-csv-dialog.component';
import {
  TransferBuilderPlansDialogComponent
} from './components/dialogs/transfer-builder-plans-dialog/transfer-builder-plans-dialog.component';
import { TransferSinglePlanDialogComponent } from './components/dialogs/transfer-single-plan-dialog/transfer-single-plan-dialog.component';
import { TransferProjectDialogComponent } from './components/dialogs/transfer-project-dialog/transfer-project-dialog.component';
import { SimpleInformationDialogComponent } from './components/dialogs/simple-information-dialog/simple-information-dialog.component';
import { AiepFormComponent } from './components/forms/Aiep-form/Aiep-form.component';
import { SortComponent } from './components/sort/sort.component';
import { EndUserFormComponent } from './components/forms/end-user-form/end-user-form.component';
import { AutocompleteComponent } from './components/molecules/autocomplete/autocomplete.component';
import { SelectorComponent } from './components/selector/selector.component';
import { FilterComponent } from './components/filter/filter.component';
import { CreateEditCountryDialogComponent } from './components/dialogs/create-edit-country-dialog/create-edit-country-dialog.component';
import { SystemLogDetailDialogComponent } from './components/dialogs/system-log-detail-dialog/system-log-detail-dialog.component';
import { CreateEditRegionDialogComponent } from './components/dialogs/create-edit-region-dialog/create-edit-region-dialog.component';
import { CreateEditAreaDialogComponent } from './components/dialogs/create-edit-area-dialog/create-edit-area-dialog.component';
import { DateRangeComponent } from './components/date-range/date-range.component';
import { FileViewerDialogComponent } from './components/dialogs/file-viewer-dialog/file-viewer-dialog.component';
import { BuilderFormComponent } from './components/forms/builder-form/builder-form.component';
import { EditVersionNotesComponent } from './components/dialogs/edit-version-notes-dialog/edit-version-notes-dialog.component';
import { ConnectionIssueDialogComponent } from './components/dialogs/connection-issue-dialog/connection-issue-dialog.component';
import { BackOnlineDialogComponent } from './components/dialogs/back-online-dialog/back-online-dialog.component';
import { PlanOfflineFormComponent } from './components/forms/plan-offline-form/plan-offline-form.component';
import {
  UploadedOfflinePlansDialogComponent
} from './components/dialogs/uploaded-offline-plans-dialog/uploaded-offline-plans-dialog.component';
import { PlanPreviewOfflineDialogComponent } from './components/dialogs/plan-preview-offline-dialog/plan-preview-offline-dialog.component';
import {
  EditVersionNotesOfflineComponent
} from './components/dialogs/edit-version-notes-offline-dialog/edit-version-notes-offline-dialog.component';
import { EndUserResultComponent } from './components/item-list/end-user-result/end-user-result.component';
import { ErrorMessageDialogComponent } from './components/dialogs/eroor-message-dialog/error-message-dialog.component';
import { CreateButtonComponent } from './components/atoms/create-button/create-button.component';
import { SpinnerComponent } from './components/atoms/spinner/spinner.component';
import { ModalComponent } from './components/atoms/modal/modal.component';
import { RadioButtonComponent } from './components/atoms/radio-button/radio-button.component';
import { MatRadioModule } from '@angular/material/radio';
import { IconComponent } from './components/atoms/icon/icon.component';
import { ButtonFilterComponent, ButtonFilterGroup } from './components/atoms/button-filter/button-filter.component';
import { SearchComponent } from './components/atoms/search/search.component';

import { AvatarComponent } from './components/atoms/avatar/avatar.component';
import { LabelComponent } from './components/atoms/label/label.component';
import { ButtonComponent } from './components/atoms/button/button.component';
import { TabsComponent } from './components/atoms/tabs/tabs.component';
import { TopbarMenuModule } from './components/molecules/topbar-menu/topbar-menu.module';

import { SidebarModule } from './components/sidebar/sidebar.module';
import { MenuComponent } from './components/organisms/menu/menu.component';
import { MenuDetailsComponent } from './components/molecules/menu-details/menu-details.component';

import { MatMenuModule } from '@angular/material/menu';
import { SelectorCountryComponent } from './components/selector-country/selector-country.component';
import { OfflineDialogComponent } from './components/organisms/dialogs/offline-dialog/offline-dialog.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { TableComponent } from './components/organisms/table/table.component';
import { TableActionsComponent } from './components/organisms/table/table-actions/table-actions.component';
import { TableActionComponent } from './components/organisms/table/table-action/table-action.component';
import { SortMenuComponent } from './components/molecules/sort-menu/sort-menu.component';
import { TableService } from './components/organisms/table/table.service';
import { InputComponent } from './components/atoms/input/input.component';
import { QuickPreviewComponent } from './components/organisms/dialogs/quick-preview/quick-preview.component';
import { CustomerHeaderComponent } from './components/molecules/customer-header/customer-header.component';
import { TableMoreActionsComponent } from './components/organisms/table/table-more-actions/table-more-actions.component';
import { TableMoreActionComponent } from './components/organisms/table/table-more-action/table-more-action.component';
import { TextAreaComponent } from "./components/atoms/text-area/text-area.component";
import { CustomerNotesComponent } from './components/molecules/customer-notes/customer-notes.component';
import { CustomerNotesExpandedComponent } from './components/molecules/customer-notes/customer-notes-expanded/customer-notes-expanded.component';
import { DetailsTemplateComponent } from './components/templates/details-template/details-template.component';
import { ClickStopPropagationDirective } from './directives/propagation.directive';
import { InfoDialogComponent } from './components/organisms/dialogs/information-dialog/information-dialog.component';
import { ItemDetailsComponent } from './components/atoms/item-details/item-details.component';
import { AssignPlanDialogComponent } from './components/organisms/dialogs/assign-plan/assign-plan-dialog.component';
import { CustomerContainerLeftHandSideComponent } from './components/organisms/customer-container-left-hand-side/customer-container-left-hand-side.component';
import { CustomerInfoComponent } from './components/molecules/customer-info/customer-info.component';
import { CustomerFormComponent } from './components/molecules/customer-form/customer-form.component';
import { EndUserDialogComponent } from './components/organisms/dialogs/end-user-dialog/end-user-dialog.component';
import { EndUserFormDialogComponent } from './components/organisms/dialogs/end-user-dialog/end-user-form-dialog/end-user-form-dialog.component';
import { DropdownDirective } from './directives/dropdown.directive';
import { DetailsContainerLeftHandSideComponent } from './components/organisms/details-container-left-hand-side/details-container-left-hand-side.component';
import { DetailsContainerTenderpackLeftHandSideComponent } from './components/organisms/details-container-tenderpack-left-hand-side/details-container-tenderpack-left-hand-side.component';
import { PlanContainerLeftHandSideComponent } from './components/organisms/details-container-left-hand-side/plan-container-left-hand-side/plan-container-left-hand-side.component';
import { SelectComponent } from './components/atoms/select/select.component';
import { PlanPreviewContainerComponent } from './components/organisms/plan-preview-container/plan-preview-container.component';
import { ProjectWithoutEndUserComponent } from './components/organisms/project-without-end-user/project-without-end-user.component';
import { PlanPublishedDialogComponent } from './components/organisms/dialogs/plan-published-dialog/plan-published-dialog.component';
import { InformationPanelComponent } from './components/atoms/information-panel/information-panel.component';
import { PlanSummaryComponent } from './components/organisms/plan-summary/plan-summary.component';
import { ProjectOverviewComponent } from './components/organisms/project-overview/project-overview.component';
import { EndUserContainerLeftHandSideComponent } from './components/organisms/details-container-left-hand-side/end-user-container-left-hand-side/end-user-container-left-hand-side.component';
import { IconsModule } from './components/atoms/icons/icons.module';
import { TradeCustomerFoundDialogComponent } from './components/organisms/dialogs/trade-customer-found-dialog/trade-customer-found-dialog.component';
import { ProjectContainerComponent } from './components/molecules/project-container/project-container.component';
import { TimelineCommentFormComponent } from './components/molecules/timeline-comment-form/timeline-comment-form.component';
import { TimelineSystemLogComponent } from './components/atoms/timeline-system-log/timeline-system-log.component';
import { SortTimelineItemsPipe } from './components/organisms/timeline/sort-timeline-items.pipe';
import { PlanDetailsMiddleContainerComponent } from './components/organisms/plan-details-middle-container/plan-details-middle-container.component';
import { PlanFormComponent } from './components/molecules/forms/plan-form/plan-form.component';
import { MultiTableComponent } from './components/molecules/multi-table/multi-table.component';
import { TopBannerComponent } from './components/atoms/top-banner/top-banner.component';
import { MainPlanDetailsFormComponent } from './components/molecules/forms/main-plan-details-form/main-plan-details-form.component';
import { NoMatchesFoundDialogComponent } from './components/organisms/dialogs/no-matches-found-dialog/no-matches-found-dialog.component';
import { TradeCustomerFormComponent } from './components/molecules/forms/trade-customer-form/trade-customer-form.component';
import { MatButtonModule } from '@angular/material/button';
import { DatePickerHeaderComponent } from './components/molecules/date-picker-header/date-picker-header.component';
import { DatePickerComponent } from './components/molecules/date-picker/date-picker.component';
import { MainPlanDetailsDialogComponent } from './components/organisms/dialogs/main-plan-details-dialog/main-plan-details-dialog.component';
import { AccountLookUpComponent } from './components/organisms/account-look-up/account-look-up.component';
import { AccountDetailsComponent } from './components/organisms/account-look-up/account-details/account-details.component';
import { DatePickerRangeComponent } from './components/molecules/date-picker-range/date-picker-range.component';
import { BackWithTitleComponent } from './components/molecules/back-with-title/back-with-title.component';
import { FilterHeaderComponent } from './components/organisms/filter-header/filter-header.component';
import { AiepDetailsFormComponent } from './components/molecules/forms/Aiep-details-form/Aiep-details-form.component';
import { CardComponent } from './components/atoms/cards/card.component';
import { RouteCardComponent } from './components/atoms/route-card/route-card.component';
import { RouterModule } from '@angular/router';
import { ContentSubheaderComponent } from './components/organisms/content-subheader/content-subheader.component';
import { UnableSupportsLogDialogComponent } from './components/organisms/dialogs/unable-supports-log-dialog/unable-supports-log-dialog.component';
import { CreatePlanOfflineComponent } from './components/organisms/create-plan-offline/create-plan-offline.component';
import { CreateNewPlanComponent } from './components/organisms/create-new-plan/create-new-plan.component';
import { CreateNewTemplateComponent } from './components/organisms/create-new-template/create-new-template.component';
import { PlanFormOfflineComponent } from './components/molecules/plan-form-offline/plan-form-offline.component';
import { HousingTypePlanFormComponent } from './components/molecules/forms/housing-type-plan-form/housing-type-plan-form.component';
import { NewTemplateFormComponent } from './components/molecules/forms/new-template-form/new-template-form.component';
import { OfflinePlanContainerLeftHandSideComponent } from './components/organisms/offline-details-container-left-hand-side -/offline-plan-container-left-hand-side/offline-plan-container-left-hand-side.component';
import { OfflineDetailsContainerLeftHandSideComponent } from './components/organisms/offline-details-container-left-hand-side -/offline-details-container-left-hand-side.component';
import { PartialAddressFormComponent } from './components/organisms/partial-address-form/partial-address-form.component';
import {
  UserCreateAndDetailsFormComponent
} from './components/molecules/forms/user-create-and-details-form/user-create-and-details-form.component';
import { UnassignTradeCustomerDialogComponent } from './components/dialogs/unassign-trade-customer-dialog/unassign-trade-customer-dialog.component';
import { UnableAutosaveRecoverDialogComponent } from './components/dialogs/unable-autosave-recover/unable-autosave-recover.component';
import { TenderPackPlanPublishComponent } from './components/dialogs/tenderPack-plan-publish/tenderPack-plan-publish.component';
import { ClickOutsideDirective } from './directives/click-outside.directive';
import { StatusLabelComponent } from './components/atoms/status-label/status-label.component';
import {
  ThreeDcUnavailableDialogComponent
} from './components/dialogs/three-dc-unavailable-dialog/three-dc-unavailable-dialog.component';
import {
  PublishPlanErrorDialogComponent
} from './components/dialogs/publish-plan-error-dialog/publish-plan-error-dialog.component';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        TranslateModule,
        MatDialogModule,
        MatAutocompleteModule,
        MatProgressSpinnerModule,
        MatCheckboxModule,
        MatTabsModule,
        MatIconModule,
        MatButtonModule,
        MatSelectModule,
        MatMenuModule,
        MatDatepickerModule,
        PdfViewerModule,
        MatTooltipModule,
        MatChipsModule,
        MatRadioModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        PipeModule,
        FormsModule,
        SidebarModule,
        TopbarMenuModule,
        IconsModule,
        RouterModule
    ],
    declarations: [
        TimelineSystemLogComponent,
        PermissionDirective,
        DndZoneDirective,
        ClickStopPropagationDirective,
        ClickOutsideDirective,
        TimelineCommentComponent,
        AutocompleteComponent,
        TextareaComponent,
        PostcodeComponent,
        SelectorComponent,
        SelectorCountryComponent,
        PlanPreviewComponent,
        PlanPublishComponent,
        TenderPackPlanPublishComponent,
        BuilderSearchResultComponent,
        PlanSearchResultComponent,
        PlatformElementDirective,
        ConfirmationDialogComponent,
        SimpleArchiveDialogComponent,
        AssignPermissionsDialogComponent,
        AssignEntitiesComponent,
        PdfViewerDialogComponent,
        UploadPlanDialogComponent,
        ExistingBuilderDialogComponent,
        InformationDialogComponent,
        UnassignedPlanDialogComponent,
        UploadCSVDialogComponent,
        UploadReleaseNotesDialogComponent,
        AiepFormComponent,
        TransferBuilderPlansDialogComponent,
        TransferSinglePlanDialogComponent,
        TransferProjectDialogComponent,
        SimpleInformationDialogComponent,
        SortComponent,
        FilterComponent,
        EndUserFormComponent,
        BuilderFormComponent,
        CreateEditCountryDialogComponent,
        CreateEditRegionDialogComponent,
        CreateEditAreaDialogComponent,
        SystemLogDetailDialogComponent,
        DateRangeComponent,
        FileViewerDialogComponent,
        EditVersionNotesComponent,
        EditVersionNotesOfflineComponent,
        ConnectionIssueDialogComponent,
        BackOnlineDialogComponent,
        PlanOfflineFormComponent,
        UploadedOfflinePlansDialogComponent,
        PlanPreviewOfflineDialogComponent,
        ErrorMessageDialogComponent,
        EndUserResultComponent,
        CreateButtonComponent,
        SpinnerComponent,
        ModalComponent,
        RadioButtonComponent,
        IconComponent,
        ButtonFilterGroup,
        ButtonFilterComponent,
        AvatarComponent,
        LabelComponent,
        ButtonComponent,
        TabsComponent,
        SearchComponent,
        OfflineDialogComponent,
        MenuComponent,
        MenuDetailsComponent,
        SearchComponent,
        TableComponent,
        TableActionsComponent,
        TableActionComponent,
        TableMoreActionsComponent,
        TableMoreActionComponent,
        SortMenuComponent,
        InputComponent,
        QuickPreviewComponent,
        CustomerHeaderComponent,
        SearchComponent,
        TextAreaComponent,
        InputComponent,
        CustomerNotesComponent,
        CustomerNotesExpandedComponent,
        AssignPlanDialogComponent,
        DetailsTemplateComponent,
        InfoDialogComponent,
        ItemDetailsComponent,
        CustomerContainerLeftHandSideComponent,
        CustomerInfoComponent,
        CustomerFormComponent,
        EndUserDialogComponent,
        EndUserFormDialogComponent,
        DetailsContainerLeftHandSideComponent,
        DetailsContainerTenderpackLeftHandSideComponent,
        PlanContainerLeftHandSideComponent,
        SelectComponent,
        DropdownDirective,
        PlanPreviewContainerComponent,
        ProjectWithoutEndUserComponent,
        PlanPublishedDialogComponent,
        InformationPanelComponent,
        PlanSummaryComponent,
        ProjectOverviewComponent,
        ProjectContainerComponent,
        EndUserContainerLeftHandSideComponent,
        TradeCustomerFoundDialogComponent,
        PlanDetailsMiddleContainerComponent,
        PlanFormComponent,
        MultiTableComponent,
        TimelineCommentFormComponent,
        SortTimelineItemsPipe,
        TopBannerComponent,
        MainPlanDetailsFormComponent,
        NoMatchesFoundDialogComponent,
        TradeCustomerFormComponent,
        DatePickerComponent,
        DatePickerHeaderComponent,
        MainPlanDetailsDialogComponent,
        AccountLookUpComponent,
        AccountDetailsComponent,
        AiepDetailsFormComponent,
        DatePickerRangeComponent,
        BackWithTitleComponent,
        FilterHeaderComponent,
        CardComponent,
        RouteCardComponent,
        ContentSubheaderComponent,
        UnableSupportsLogDialogComponent,
        CreatePlanOfflineComponent,
        CreateNewPlanComponent,
        CreateNewTemplateComponent,
        PlanFormOfflineComponent,
        HousingTypePlanFormComponent,
        NewTemplateFormComponent,
        OfflinePlanContainerLeftHandSideComponent,
        OfflineDetailsContainerLeftHandSideComponent,
        PartialAddressFormComponent,
        UserCreateAndDetailsFormComponent,
        UnassignTradeCustomerDialogComponent,
        UnableAutosaveRecoverDialogComponent,
        StatusLabelComponent,
      ThreeDcUnavailableDialogComponent,
      PublishPlanErrorDialogComponent
    ],
    exports: [
        CommonModule,
        ReactiveFormsModule,
        TranslateModule,
        PermissionDirective,
        ClickStopPropagationDirective,
        ClickOutsideDirective,
        DndZoneDirective,
        MatDialogModule,
        IconsModule,
        DatePickerComponent,
        BuilderSearchResultComponent,
        PlanSearchResultComponent,
        AssignEntitiesComponent,
        TimelineCommentComponent,
        TimelineCommentFormComponent,
        AutocompleteComponent,
        TextareaComponent,
        PostcodeComponent,
        SelectorComponent,
        SelectorCountryComponent,
        AiepFormComponent,
        EndUserFormComponent,
        BuilderFormComponent,
        SortComponent,
        FilterComponent,
        DateRangeComponent,
        EditVersionNotesComponent,
        EditVersionNotesOfflineComponent,
        PlanOfflineFormComponent,
        EndUserResultComponent,
        CreateButtonComponent,
        SpinnerComponent,
        ModalComponent,
        RadioButtonComponent,
        IconComponent,
        ButtonFilterGroup,
        ButtonFilterComponent,
        AvatarComponent,
        LabelComponent,
        ButtonComponent,
        TabsComponent,
        SearchComponent,
        OfflineDialogComponent,
        MenuComponent,
        SearchComponent,
        TableComponent,
        TableActionsComponent,
        TableActionComponent,
        TableMoreActionsComponent,
        TableMoreActionComponent,
        SortMenuComponent,
        InputComponent,
        CustomerContainerLeftHandSideComponent,
        CustomerHeaderComponent,
        DetailsTemplateComponent,
        QuickPreviewComponent,
        SearchComponent,
        TextAreaComponent,
        CustomerNotesComponent,
        InfoDialogComponent,
        EndUserDialogComponent,
        EndUserFormDialogComponent,
        ItemDetailsComponent,
        AssignPlanDialogComponent,
        DropdownDirective,
        DetailsContainerLeftHandSideComponent,
        DetailsContainerTenderpackLeftHandSideComponent,
        PlanContainerLeftHandSideComponent,
        PlanPreviewContainerComponent,
        ProjectWithoutEndUserComponent,
        SelectComponent,
        ProjectContainerComponent,
        PlanPublishedDialogComponent,
        PlanSummaryComponent,
        ProjectOverviewComponent,
        InformationPanelComponent,
        EndUserContainerLeftHandSideComponent,
        TradeCustomerFoundDialogComponent,
        PlanDetailsMiddleContainerComponent,
        PlanFormComponent,
        MultiTableComponent,
        SortTimelineItemsPipe,
        TopBannerComponent,
        MainPlanDetailsFormComponent,
        NoMatchesFoundDialogComponent,
        TradeCustomerFormComponent,
        MainPlanDetailsDialogComponent,
        AccountLookUpComponent,
        AccountDetailsComponent,
        AiepDetailsFormComponent,
        DatePickerRangeComponent,
        BackWithTitleComponent,
        FilterHeaderComponent,
        CardComponent,
        RouteCardComponent,
        ContentSubheaderComponent,
        CreatePlanOfflineComponent,
        CreateNewPlanComponent,
        CreateNewTemplateComponent,
        PlanFormOfflineComponent,
        HousingTypePlanFormComponent,
        NewTemplateFormComponent,
        OfflinePlanContainerLeftHandSideComponent,
        OfflineDetailsContainerLeftHandSideComponent,
        PartialAddressFormComponent,
        UserCreateAndDetailsFormComponent,
        TimelineSystemLogComponent,
        UnassignTradeCustomerDialogComponent,
        UnableAutosaveRecoverDialogComponent,
        StatusLabelComponent,
        ThreeDcUnavailableDialogComponent,
        PublishPlanErrorDialogComponent
    ],
    providers: [
        TableService,
        { provide: MAT_TAB_GROUP, useValue: {} }
    ]
})
export class SharedModule { }


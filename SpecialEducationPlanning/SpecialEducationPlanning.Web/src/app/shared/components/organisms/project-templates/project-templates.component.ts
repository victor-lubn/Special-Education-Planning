import { CdkScrollable, ScrollDispatcher } from '@angular/cdk/scrolling';
import { AfterViewInit, Component, ElementRef, Input, NgZone, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TableColumnConfig, TableRecords } from 'src/app/shared/components/organisms/table/table.types';
import { Project } from 'src/app/shared/models/project';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { Plan } from 'src/app/shared/models/plan';
import { ProjectTemplates } from 'src/app/shared/models/project-templates.model';
import { EducationToolService } from '../../../../core/Education-tool/Education-tool.service';
import { EducationToolType } from '../../../models/app-enums';


@Component({
  selector: 'tdp-project-templates',
  templateUrl: './project-templates.component.html',
  styleUrls: ['./project-templates.component.scss'],
})
export class ProjectTemplatesComponent implements OnInit, AfterViewInit {

  @Input() project: Project;
  @Input() plans: Plan[] = [];
  public templates: TableRecords<any>;
  public projectTemplatesColumnNames: TableColumnConfig[] = [];
  public minimizeHeader = false;
  public name: string;
  public housingSpec: string;
  private nameHeader: string = '';
  private housingSpecHeader: string = '';
  public templatesAndPlansString: string;
  public containerHeight: string = '400px'
  @ViewChild("header", { static: false }) header: ElementRef;
  @ViewChild("tableContainer", { static: false }) tableContainer: ElementRef;

  EducationOriginType = EducationToolType;

  constructor(
    private translate: TranslateService,
    private scrollDispatcher: ScrollDispatcher,
    private zone: NgZone,
    private dialogsService: DialogsService,
    protected EducationToolService: EducationToolService
  ) {
    this.name = '';
    this.housingSpec = '';
   }

  ngOnInit(): void {
    this.initializeTranslationStrings();
    this.initializeColumnConfiguration();
    this.setPlansTableRecords(this.plans);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.plans && changes.plans.currentValue) {
      this.setPlansTableRecords(this.plans);
    }
  }

  private setPlansTableRecords(plans: Plan[]): void {
    const uniquePlans = plans.filter((plan, index, self) =>
      index === self.findIndex((p) => p.id === plan.id)
    );
    this.templates = {
      data: this.project?.projectTemplates.map(template => {
        const plan = uniquePlans.find(p => p.id === template.planId);
        template.plan = plan;
        return {
          ...template,
          name: plan?.planName,
          housingSpec: plan?.housingSpecificationTemplatesModel?.housingSpecificationModel?.name
          ?? plan?.housingSpecificationTemplatesModel?.housingSpecification?.name
        };
      })
    };
  }

  private initializeTranslationStrings() {
    this.translate.get([
      'projectTemplates.name',
      'projectTemplates.housingSpec',
      'projectTemplates.templatesAndPlans'
    ]).subscribe((translations) => {
      this.templatesAndPlansString = translations['projectTemplates.templatesAndPlans'];
      this.nameHeader = translations['projectTemplates.name'];
      this.housingSpecHeader = translations['projectTemplates.housingSpec'];
    });
  }

  private initializeColumnConfiguration() {
    this.projectTemplatesColumnNames = [
      { columnDef: 'name', header: this.nameHeader, tooltipAtLength: 25 },
      { columnDef: 'housingSpec', header: this.housingSpecHeader, tooltipAtLength: 25 },
    ]
  }

  ngAfterViewInit() {
    this.scrollDispatcher.scrolled().subscribe((cdk: CdkScrollable) => {
      this.zone.run(() => {
        const scrollPosition = cdk?.getElementRef().nativeElement.scrollTop;
        if (this.header && scrollPosition > this.header.nativeElement.offsetHeight) {
          this.minimizeHeader = true;
        }
      });
    });
    this.calculateContainerHeight();
  }

  editInEducationTool(event: MouseEvent, template: ProjectTemplates): void {
    event.stopPropagation();
    event.preventDefault();
    if (template.plan && template.plan.masterVersion) {
      this.EducationToolService.openVersionInEducationTool(template.plan.masterVersion, template.plan.builderId, template.plan.EducationOrigin);
      this.calculateContainerHeight();
    }
    else {
      this.dialogsService.simpleInformation('dialog.emptyTemplateTitle', 'dialog.emptyTemplateMsg');
    }
  }

  createTemplate() {
    this.dialogsService.openCreateNewTemplateModal({ data: null, project: this.project, projectWide : true, navigation: false});
  }

  private calculateContainerHeight(): void {
    setTimeout(() => {
      const rowElements = this.tableContainer.nativeElement.querySelectorAll('mat-row');
      const totalHeight = Array.from(rowElements).reduce((acc: number, row) => acc + (row as HTMLElement).offsetHeight, 0) as number;
      const maxHeight = 600;
      this.containerHeight = totalHeight > maxHeight ? `${maxHeight}px` : `${totalHeight + 200}px`;
    });
  }
}


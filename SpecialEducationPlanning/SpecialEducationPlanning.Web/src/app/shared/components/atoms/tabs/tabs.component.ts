import { AfterViewInit, ChangeDetectorRef, Component, ContentChildren, ElementRef, EventEmitter, OnInit, Output, QueryList, ViewChild, ViewChildren, ViewEncapsulation } from '@angular/core';
import { MatTab, MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';


@Component({
  selector: 'tdp-tabs',
  templateUrl: './tabs.component.html',
  styleUrls: ['./tabs.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TabsComponent implements AfterViewInit {
  @ViewChild('matTabGroup', { static: false }) public matTabGroup: MatTabGroup;
  @ViewChildren(MatTab) inclusiveTabs: QueryList<MatTab>;
  @ContentChildren(MatTab, { descendants: true }) tabsFromNgContent: QueryList<MatTab>;

  @Output() onTabClicked = new EventEmitter<number>();

  public singleTab: boolean

  constructor(private cdr: ChangeDetectorRef) {

  }

  ngAfterViewInit() {
    const tabs = [...this.inclusiveTabs.toArray(), ...this.tabsFromNgContent.toArray()];

    this.matTabGroup._tabs.reset(tabs);
    this.matTabGroup._tabs.notifyOnChanges();

    if (tabs.length <= 1) {
      this.singleTab = true;
      this.cdr.detectChanges()
    }
  }

  public getCurrentTabIndex(tabChangeEvent: MatTabChangeEvent){
    const currentTabIndex = tabChangeEvent.index;
    this.onTabClicked.emit(currentTabIndex);
  }
}

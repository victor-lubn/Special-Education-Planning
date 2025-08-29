import { CommonModule } from '@angular/common';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { TabsComponent } from './tabs.component';


export default {
    component: TabsComponent,
    decorators: [
        moduleMetadata({
            declarations: [TabsComponent],
            imports: [CommonModule, MatTabsModule, BrowserAnimationsModule,
                BrowserModule
            ],
            providers: [
                { provide: MAT_TAB_GROUP, useValue: {} },
            ]
        })
    ],
    title: 'Atom/Tabs'
} as Meta;

const Template: Story<TabsComponent> = (args) => ({
    props: {
        ...args
    },
    template: `<tdp-tabs><mat-tab>
    <ng-template mat-tab-label class="tdp-atom-label">    
      First <div class="tdp-atom-tab-count">22</div>
    </ng-template>
    Content 1
  </mat-tab><mat-tab>
  <ng-template mat-tab-label class="tdp-atom-label">    
    Second <div class="tdp-atom-tab-count">222</div>
  </ng-template>  
  Content 2
</mat-tab><mat-tab>
<ng-template mat-tab-label class="tdp-atom-label">    
  Second <div class="tdp-atom-tab-count">222</div>
</ng-template>  
Content 2
</mat-tab></tdp-tabs>`,
});

export const TabsBar = Template.bind({});


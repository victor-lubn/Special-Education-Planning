import { SidebarComponent } from './sidebar.component';
import { IconComponent } from '../atoms/icon/icon.component';
import { ResizableDirective} from './resizable.directive';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SidebarButtonTestComponent } from './sidebar-button-test.component';

export default {
  component: SidebarComponent,
  decorators: [
    moduleMetadata({
      declarations: [SidebarComponent, ResizableDirective, IconComponent, SidebarButtonTestComponent],
      imports: [CommonModule, BrowserAnimationsModule],
    }),
  ],
  title: 'Components/Sidebar',
} as Meta;

const Template: Story<SidebarComponent> = args => ({
  props: {
    ...args
  },
  template: `
  <div style="width: 1600px; height: 500px; position: relative; border: 1px solid red;">    
      <sidebar-test-button *ngIf="position === 'right'" sidebar="rightSidebar"></sidebar-test-button>
      <tdp-sidebar tdpResizable [name]="name" [position]="position"
          [folded]="folded" [foldedWidth]="200"
          [lockedOpen]="lockedOpen" [invisibleOverlay]="invisibleOverlay">
          <p>Lorem ipsum, dolor sit amet consectetur adipisicing elit. Iusto, officiis.</p>
          <p>Sapiente fugiat quam itaque omnis aliquam quaerat minima, quo temporibus.</p>
          <p>Laborum deleniti non molestiae animi laboriosam ab provident blanditiis expedita.</p>
          <p>Pariatur inventore praesentium consectetur alias odio cupiditate quaerat cum officiis!</p>
          <p>Sapiente ad aspernatur consectetur? Placeat modi laborum in id facere.</p>
          <p>Omnis modi, veritatis tempore eius veniam beatae? Maiores, quisquam optio.</p>
          <p>Facilis odit ducimus magni eaque, iste corporis quam incidunt officiis.</p>
          <p>Amet culpa libero aspernatur, quam omnis quis in optio debitis?</p>
          <p>Corrupti exercitationem similique asperiores suscipit? Error dolores sint iure facilis!</p>
          <p>Aut odit dolor consectetur magnam voluptatibus vitae deleniti voluptatem quos?</p>
      </tdp-sidebar>
      <test-button style="position: absolute; right: 0;" *ngIf="position === 'left'" sidebar="leftSidebar"></test-button>
    </div>
  `,
});

export const Sidebar = Template.bind({});

Sidebar.args = {
  name: 'rightSidebar',
  position: 'right'
};

export const SidebarLeft = Template.bind({});

SidebarLeft.args = {
  name: 'leftSidebar',
  position: 'left'
};

export const SidebarOpened = Template.bind({});

SidebarOpened.args = {
  name: 'rightSidebar',
  folded: true,
  lockedOpen: true,
  opened: true,
  position: 'right',
  invisibleOverlay: true
};


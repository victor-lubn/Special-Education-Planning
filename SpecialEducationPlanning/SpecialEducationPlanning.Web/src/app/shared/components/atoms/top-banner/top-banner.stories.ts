import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { TopBannerComponent } from './top-banner.component';
import { IconComponent } from '../icon/icon.component';

export default {
    component: TopBannerComponent,
    decorators: [
        moduleMetadata({
            declarations: [TopBannerComponent, IconComponent],
            imports: [CommonModule]
        })
    ],
    argTypes: {
        onCheck: {
            action: 'onCheckedChange'
        }
    },
    title: 'Atom/Top banner'
} as Meta;

const Template: Story<TopBannerComponent> = (args, template) => ({
    props: {
        ...args
    },
    template: `<tdp-top-banner [informative]="informative" [error]="error" [offline]="offline">Top banner</tdp-top-banner>`
});
export const DefaultTopBanner = Template.bind({});
DefaultTopBanner.args = {
}


export const OfflineTopBanner = Template.bind({});
OfflineTopBanner.args = {
    offline: true
}

export const InformativeTopBanner = Template.bind({});
InformativeTopBanner.args = {
    offline: true,
    informative: true
}

export const ErrorTopBanner = Template.bind({});
ErrorTopBanner.args = {
    offline: true,
    error: true
}

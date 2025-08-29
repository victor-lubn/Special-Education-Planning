import { Meta, Story } from "@storybook/angular";

export default {
    title: 'Atom/Scrollbar'
} as Meta;

const Template: Story = () => ({
    template: `
    <div style="height: 150px; width: 150px; overflow: scroll">
        <p>
            Tab Scrollbar
        </p>
        <p>
            A Tab Scroller allows for smooth native and animated scrolling of tabs.
        </p>
        <p>
            When a screen scrolls, tabs can either be fixed to the top of the screen, or scroll off the screen. If they scroll off the screen, they will return when the user scrolls upward.
        </p>
    </div>
    `
});

export const Scrollbar = Template.bind({});
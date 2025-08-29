import { Meta, Story } from "@storybook/angular";

export default {
    title: 'Atom/Typography'
} as Meta;

const Template: Story = (args) => ({
    props: args,
    template: `<${args.element}>Heading text</${args.element}>`
});

export const Heading1 = Template.bind({});
Heading1.args = {
    element: 'h1'
};

export const Heading2 = Template.bind({});
Heading2.args = {
    element: 'h2'
};

export const Heading3 = Template.bind({});
Heading3.args = {
    element: 'h3'
};

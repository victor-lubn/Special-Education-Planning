import { Meta, Story } from "@storybook/angular";

export default {
    title: 'Atom/Typography'
} as Meta;

const Template: Story = (args) => ({
    props: args,
    template: 
        `<div [class]="className">
            <p>Body desktop</p> 
            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed et enim ut mi venenatis hendrerit ac nec nisl. Aenean aliquam congue arcu sed porta. Suspendisse sodales, felis nec viverra luctus, nisl risus lacinia diam, vel ultricies metus sapien ut odio.</p>
        </div>`
});

export const BodyNormal = Template.bind({});
BodyNormal.args = {
    className: 'body--size-normal'
};

export const BodySmall = Template.bind({});
BodySmall.args = {
    className: 'body--size-small'
};
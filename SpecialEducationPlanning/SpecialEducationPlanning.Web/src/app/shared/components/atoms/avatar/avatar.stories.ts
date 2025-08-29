import { AvatarComponent } from './avatar.component';
import { moduleMetadata, Meta, StoryObj, argsToTemplate } from '@storybook/angular';
import { CommonModule } from '@angular/common';

const meta: Meta<AvatarComponent> = {
  title: 'Atom/Avatar',
  component: AvatarComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [CommonModule],
    }),
  ],
  argTypes : {
    initials: {
      name: 'initials',
      type: { name: 'string', required: false },
      defaultValue: 'LB',
      description: 'User can his initials to be shown in avatar',
    },
    sizeAvatar: {
      name: 'sizeAvatar',
      type: { name: 'string', required: false },
      options: ["small", "large",],
      control: { type: 'radio' },
      defaultValue: 'small',
      description: 'User can select the size of the avatar from the options',
    },
    onClick: {
      action: ('clicked')
    },
  },
  render: (args: AvatarComponent) => ({
    props: {
      ...args,
    },
    template: `<tdp-avatar ${argsToTemplate(args)}></tdp-avatar>`
  }),
}

export default meta;
type Story = StoryObj<AvatarComponent>;


export const LargeAvatarWithInitials: Story = {
  args: {
    initials: 'LB',
    sizeAvatar: 'large',
  }
}

export const SmallAvatarWithInitials: Story = {
  args: {
    ...LargeAvatarWithInitials.args,
    sizeAvatar: 'small'
  }
}
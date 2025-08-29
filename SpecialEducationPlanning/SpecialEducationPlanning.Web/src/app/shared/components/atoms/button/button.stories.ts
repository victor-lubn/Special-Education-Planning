import { ButtonComponent } from './button.component';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';


export default {
  component: ButtonComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      declarations: [ButtonComponent],
      imports: [CommonModule],
    }),
  ],
  argTypes: {
    size: {
      name: 'size',
      type: { name: 'string', required: false },
      options: ["small", "medium", "large", "xl"],
      control: { type: 'radio' },
      defaultValue: 'small',
      description: 'User can select the size of the button from the options',
    },
    color: {
      name: 'color',
      type: { name: 'string', required: false },
      options: ["navy", "white", "green", "text-only"],
      control: { type: 'radio' },
      defaultValue: 'white',
      description: 'User can select the color of the button from the options'
    },
    icon: {
      name: 'icon',
      type: { name: 'string', required: false },
      defaultValue: '',
      description: 'Optional icon that can be added to the content of the button'
    }
  },

  title: 'Atom/Button',
} as Meta;



const Template: Story<ButtonComponent> = args => ({
  props: {
    ...args,

  },
  template: `<tdp-button [color]="color" [size]="size" [disabled]="disabled"><div title>Example button</div></tdp-button>`,
});

const IconButtonTemplate: Story<ButtonComponent> = args => ({
  props: {
    ...args,

  },
  template: `<tdp-button [color]="color" [size]="size" [icon]="icon" [whiteBlueButton]="whiteBlueButton" [tooltip]="tooltip" [disabled]="disabled"><i class="fas fa-address-card" icon-button></i> <div title>Example button</div></tdp-button>`,
});

const TooltipButtonTemplate: Story<ButtonComponent> = args => ({
  props: {
    ...args,

  },
  template: `<tdp-button [color]="color" [size]="size" [icon]="icon" [whiteBlueButton]="whiteBlueButton" [tooltip]="tooltip" [disabled]="disabled"><div tooltip>{{tooltip}}</div><i class="fas fa-address-card" icon-button></i> <div title>Example button</div></tdp-button>`,
});


// Small buttons
export const WhiteSmall = Template.bind({});
WhiteSmall.args = {
  color: 'white',
  size: 'small'
};

export const NavySmall = Template.bind({});
NavySmall.args = {
  color: 'navy',
  size: 'small'
};

export const GreenSmall = Template.bind({});
GreenSmall.args = {
  color: 'green',
  size: 'small'
};

export const TextOnlySmall = Template.bind({});
TextOnlySmall.args = {
  color: 'text-only',
  size: 'small'
};

export const WhiteSmallDisabled = Template.bind({});
WhiteSmallDisabled.args = {
  color: 'white',
  size: 'small',
  disabled: true
};

export const NavySmallDisabled = Template.bind({});
NavySmallDisabled.args = {
  color: 'navy',
  size: 'small',
  disabled: true
};

export const GreenSmallDisabled = Template.bind({});
GreenSmallDisabled.args = {
  color: 'green',
  size: 'small',
  disabled: true
};

export const TextonlySmallDisabled = Template.bind({});
TextonlySmallDisabled.args = {
  color: 'text-only',
  size: 'small',
  disabled: true
};



// Medium buttons
export const WhiteMedium = Template.bind({});
WhiteMedium.args = {
  color: 'white',
  size: 'medium'
};

export const NavyMedium = Template.bind({});
NavyMedium.args = {
  color: 'navy',
  size: 'medium'
};

export const GreenMedium = Template.bind({});
GreenMedium.args = {
  color: 'green',
  size: 'medium'
};

export const TextOnlyMedium = Template.bind({});
TextOnlyMedium.args = {
  color: 'text-only',
  size: 'medium'
};

// Large buttons
export const WhiteLarge = Template.bind({});
WhiteLarge.args = {
  color: 'white',
  size: 'large'
};

export const NavyLarge = Template.bind({});
NavyLarge.args = {
  color: 'navy',
  size: 'large'
};

export const GreenLarge = Template.bind({});
GreenLarge.args = {
  color: 'green',
  size: 'large'
};

export const TextOnlyLarge = Template.bind({});
TextOnlyLarge.args = {
  color: 'text-only',
  size: 'large'
};

// Xl buttons
export const WhiteXl = Template.bind({});
WhiteXl.args = {
  color: 'white',
  size: 'xl'
};

export const NavyXl = Template.bind({});
NavyXl.args = {
  color: 'navy',
  size: 'xl'
};

export const GreenXl = Template.bind({});
GreenXl.args = {
  color: 'green',
  size: 'xl'
};

export const TextOnlyXl = Template.bind({});
TextOnlyXl.args = {
  color: 'text-only',
  size: 'xl'
};

export const IconButton = IconButtonTemplate.bind({});
IconButton.args = {
  color: 'green',
  size: 'xl',
  icon: true
};

export const WhiteBlueButton = IconButtonTemplate.bind({});
WhiteBlueButton.args = {
  icon: true,
  whiteBlueButton: true,
};

export const DisabledWhiteBlueButton = IconButtonTemplate.bind({});
DisabledWhiteBlueButton.args = {
  icon: true,
  whiteBlueButton: true,
  disabled: true
};

export const TooltipButton = TooltipButtonTemplate.bind({});
TooltipButton.args = {
  color: 'green',
  size: 'xl',
  icon: true,
  whiteBlueButton: true,
  tooltip: 'Using Aiep discount'
};



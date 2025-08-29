// FIXME - resolve issues with molecules and organisms components, and include them also in build
module.exports = {
  framework: {
    name: '@storybook/angular',
    options: { enableIvy: true },
  },
  stories:[
    '../src/app/shared/components/atoms/**/*.stories.ts',
  ],
  addons:['@storybook/addon-actions', '@storybook/addon-links', '@storybook/addon-essentials', '@storybook/addon-storyshots']
  }
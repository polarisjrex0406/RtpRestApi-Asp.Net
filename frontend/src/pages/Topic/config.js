import color from '@/utils/color';

export const fields = {
  name: {
    type: 'stringWithColor',
    required: true,
  },
  group: {
    type: 'textarea',
  },
  goal: {
    type: 'textarea',
  },
  topicPrompt: {
    type: 'initPrompt',
    required: true,
  },
  enabled: {
    type: 'boolean',
  },
};

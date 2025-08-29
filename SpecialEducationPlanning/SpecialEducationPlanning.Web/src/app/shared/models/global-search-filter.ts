import { HomeFilterBuilderForm } from './home-filter-builer-form';
import { HomeFilterPlanForm } from './home-filter-plan-form';
import { HomeFilterProjectForm } from './home-filter-project-form';

export interface GlobalSearchFilter {
  selected: {
    key: 'homeFilterPlanForm',
    value: HomeFilterPlanForm
  } |
  {
    key: 'homeFilterBuilderForm',
    value: HomeFilterBuilderForm
  } |
  {
    key: 'homeFilterProjectForm',
    value: HomeFilterProjectForm
  } |
  {
    key: 'omniSearch',
    value: string
  } &
  { key: 'homeFilterPlanForm' | 'homeFilterBuilderForm' | 'homeFilterProjectForm' | 'omniSearch' };
}

import { isNil } from 'lodash';
import { ResolveFilterDescriptorsMapper } from 'src/app/shared/models/ResolveFilterDescriptorsMapper';
import { FilterDescriptor } from '../services/url-parser/filter-descriptor.model';

export class TdpUtils {
  
    /**
     * Check if a value is null, undefined or empty
     *
     * @param value
     * @returns {boolean}
     */
     public static isEmpty(value: any) {
      return isNil(value) || value === '';
    }
  
    /**
     * Resolve object from a path given
     *
     * @param path ('user.name')
     * @param obj ({ user: { name: 'name'} })
     * @returns {any} ('name')
     */
    public static resolveObject<Return = any, Data extends object = {}>(path: string, obj: Data): Return | null | undefined {
      return (
        path.split('.').reduce((prev, curr) => {
          return prev ? prev[curr] : null;
        }, obj) || undefined
      );
    }

  public static resolveFilterDescriptors<Filter extends object>(opts: {
    filter: Filter;
    mappers: ResolveFilterDescriptorsMapper[];
    ignoreValues?: any[];
  }): FilterDescriptor[] {
    const {
      mappers = [],
      filter,
      ignoreValues = ['', null]
    } = opts;
    const filterDescriptors: FilterDescriptor[] = [];
    mappers.forEach(mapper => {
      const { path, member, operator, resolver } = mapper;
      const valueResolved = TdpUtils.resolveObject(path, filter);
      if (valueResolved && !ignoreValues.includes(valueResolved)) {
        filterDescriptors.push({
          member,
          operator,
          value: typeof resolver === 'function' ? resolver(valueResolved) : valueResolved
        });
      }
    });
    return filterDescriptors;
  }
}

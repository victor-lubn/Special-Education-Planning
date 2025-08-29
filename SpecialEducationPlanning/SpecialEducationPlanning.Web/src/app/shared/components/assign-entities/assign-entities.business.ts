import { MatCheckboxChange } from '@angular/material/checkbox';
import { AssignEntitiesComponent } from './assign-entities.component';

export function checkInputValidations(componentScope: AssignEntitiesComponent): void {
  if (!componentScope.displayProperty) {
    throw new TypeError('The input "displayProperty" is required');
  }
  if (typeof componentScope.displayProperty !== 'string') {
    throw new TypeError('The input "displayProperty" must be a string');
  }
  if (!componentScope.assignedEntities) {
    throw new TypeError('The input "assignedEntities" is required');
  }
  if (!Array.isArray(componentScope.assignedEntities)) {
    throw new TypeError('The input "assignedEntities" must be an array');
  }
  if (!componentScope.availableEntities) {
    throw new TypeError('The input "availableEntities" is required');
  }
  if (!Array.isArray(componentScope.availableEntities)) {
    throw new TypeError('The input "availableEntities" must be an array');
  }
}

export function checkProperty(displayProperty: string, collectionName: string, entity: any): void {
  if (!entity.hasOwnProperty(displayProperty)) {
    throw new TypeError(
      `One or more entities in "${collectionName}" array does not contains the "${displayProperty}" property`
    );
  }
}

export function checkEntities(componentScope: AssignEntitiesComponent): void {
  componentScope.assignedEntities.forEach(
    checkProperty.bind(componentScope, componentScope.displayProperty, 'assignedEntities')
  );
  componentScope.availableEntities.forEach(
    checkProperty.bind(componentScope, componentScope.displayProperty, 'availableEntities')
  );
}

export function compare(displayProperty: string, a: any, b: any): number {
  if (a[displayProperty] < b[displayProperty]) {
    return -1;
  }
  if (a[displayProperty] > b[displayProperty]) {
    return 1;
  }
  return 0;
}

export function sortEntities(componentScope: AssignEntitiesComponent): void {
  componentScope.assignedEntities.sort(compare.bind(null, componentScope.displayProperty));
  componentScope.availableEntities.sort(compare.bind(null, componentScope.displayProperty));
}

export function addOrRemoveCheckedEntity(event: MatCheckboxChange, collection: any[]): void {
  if (event.checked) {
    collection.push(event.source.value);
  } else {
    collection.splice(collection.indexOf(event.source.value), 1);
  }
}

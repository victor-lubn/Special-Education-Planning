import { Component, Input, ChangeDetectionStrategy, Output, OnChanges, EventEmitter, OnInit } from '@angular/core';

import { BaseEntity } from '../../base-classes/base-entity';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { compare, addOrRemoveCheckedEntity, checkInputValidations, checkEntities, sortEntities } from './assign-entities.business';

/**
 * Component for assigning entities from the available list.
 *
 * How to use:
 *
 * We want to add de component in other component we need to pass the following variables:
 *  - updateAvailableEvent: Event when we updated the avaliable list.
 *  - updateAssignedEvent: Event when we updated the assigned list.
 *  - displayProperty: This is use for show the property in the list:
 *      for example: we have the following model
 *      [{"id":1, "name":"Assign1"},{"id":2, "name":"Assign2"},{"id":1, "name":"Assign2"}]
 *      If we want to show the "name" property or we put the string in a property of
 *      the parent component or we add it as a string in displayProperty. Up to you.
 *  - assignedEntities: Assigned entities list, (left side).
 *  - availableEntities: Avaliable entities list, (right side).
 *  - listsTitleKey: Translation Key for lists, if not provided the list only show 'Assigned' / 'Available' simple labels.
 * ```
 * <father-component>
 *
 *    <tdp-assign-entities
 *      (updateAvailableEvent)="onUpdateAvailableEvent($event)"
 *      (updateAssignedEvent)="onUpdateAssignedEvent($event)"
 *      [displayProperty]="permissionDisplayProperty"
 *      [assignedEntities]="permissionsAssigned"
 *      [availableEntities]="permissionsAvailable"
 *      [listsTitleKey]="listsTitleKey">
 *    </tdp-assign-entities>
 *
 * </father-component>
 * ```
 *
 */

export interface AssignEntitiesChangeEvent<T> {
  assigned: T[];
  available: T[];
}
@Component({
  selector: 'tdp-assign-entities',
  templateUrl: 'assign-entities.component.html',
  styleUrls: ['assign-entities.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AssignEntitiesComponent extends BaseEntity implements OnChanges {

  public assignedEntitiesChecked: any[];
  public availableEntitiesChecked: any[];
  public searchTextAssigned: string;
  public searchTextAvailable: string;

  @Output() public changedLists = new EventEmitter();
  @Input() public displayProperty: string;
  @Input() public assignedEntities: any[] = [];
  @Input() public availableEntities: any[] = [];
  @Input() public leftInputTitle: string;
  @Input() public rightInputTitle: string;
  @Input() public onlyAssignAction: boolean;

  constructor() {
    super();
    this.assignedEntitiesChecked = [];
    this.availableEntitiesChecked = [];
  }

  public ngOnChanges() {
    checkInputValidations(this);
    checkEntities(this);
    sortEntities(this);
  }

  public onAssignedOptionchecked(event: MatCheckboxChange): void {
    addOrRemoveCheckedEntity(event, this.assignedEntitiesChecked);
  }

  public onAvailabledOptionchecked(event: MatCheckboxChange): void {
    addOrRemoveCheckedEntity(event, this.availableEntitiesChecked);
  }

  public assignHandler(): void {
    this.passItems('assignedEntities', 'availableEntities', 'availableEntitiesChecked');
  }

  public unassignHandler(): void {
    this.passItems('availableEntities', 'assignedEntities', 'assignedEntitiesChecked');
  }

  private passItems(target: string, source: string, checkslist: string): void {
    if (this[checkslist] && this[checkslist].length > 0) {
      this[target] = this[target].concat(this[checkslist]);
      this[source] = this[source].filter(item => !this[checkslist].includes(item));
      this[checkslist].length = 0;
      this[target].sort(compare.bind(null, this.displayProperty));
      this.changedLists.emit({
        assigned: this.assignedEntities,
        available: this.availableEntities
      });
    }
  }

}

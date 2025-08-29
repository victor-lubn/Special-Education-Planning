import { inject } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRouteSnapshot, CanDeactivateFn, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { Observable } from 'rxjs';

import {
  ConfirmationDialogComponent,
  ConfirmationDialogData
} from '../components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { NetworkStatusService } from '../../core/services/network-status/network-status.service';


/**
 *
 * This interface must be implemented in order to use {@link PendingChangesGuard} in modules routes.
 *
 * ```ts
  export class ExampleComponent implements ComponentCanDeactivate {
    ...
    hasChanges(): boolean | Observable<boolean> {
      return booleanCondition;
    }
    ...
  }
 ```
 *
 */
export interface ComponentCanDeactivate {
  /**
   * This function evaluates if the component has changes  that could be lost.
   * returns boolean | Observable<boolean>
   */
  hasChanges: () => boolean | Observable<boolean>;
}

/**
 * This Guard allows to show a confirmation dialog before deactivating a route.
 * To use this Guard the {@link ComponentCanDeactivate} interface must be implemented by the routed component.
 * The confirmation dialog appears when the hasChanges() function is evaluated as true.
 *
 * ```ts
  const routes: Routes = [
    { path: 'componentPath', component: ExampleComponent, canDeactivate: [PendingChangesGuard] }
  ];
 ```
 */
export const pendigChangesGuard: CanDeactivateFn<ComponentCanDeactivate> = 
(component: ComponentCanDeactivate, currentRoute: ActivatedRouteSnapshot) => {

  const dialog = inject(MatDialog);
  const router = inject(Router);
  const location = inject(Location);
  const network = inject(NetworkStatusService);

  if (component.hasChanges() && !network.isReloading()) {
    const dialogRef = dialog.open<ConfirmationDialogComponent, ConfirmationDialogData, boolean>(
      ConfirmationDialogComponent,
      {
        data: {
          width: '400px',
          titleStringKey: 'dialog.genericUnsavedChanges',
          messageStringKey: 'dialog.genericCancelDialog'
        }
      }
    );
    const currentUrlTree = router.createUrlTree([], currentRoute);
    location.go(currentUrlTree.toString());
    return dialogRef.afterClosed();
  } else {
    return true;
  }
}

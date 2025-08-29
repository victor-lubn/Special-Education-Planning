import { Location } from '@angular/common';
import { Router } from '@angular/router';

import { ServiceInjector } from '../../core/services/service-injector/service-injector';
import { BaseEntity } from './base-entity';
/**
 * Base Component class
 *
 * Application components should extend this class to reuse base functionality
 */
export abstract class BaseComponent extends BaseEntity {

  protected translations: { [key: string]: string };

  private location: Location;
  private router: Router;

  constructor() {
    super();
    this.translations = {};
    this.location = ServiceInjector.injector.get(Location);
    this.router = ServiceInjector.injector.get(Router);
  }

  public goBack(): void {
    this.location.back();
  }

  public navigateTo(appRoute: string, queryParams?: any): Promise<Boolean> {
    return this.router.navigate([appRoute], { queryParams });
  }
}

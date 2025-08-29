import { Component } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Router } from '@angular/router';

import { ServiceInjector } from '../../../core/services/service-injector/service-injector';
import { environment } from '../../../app.module';

@Component({
  selector: 'tdp-jobs',
  templateUrl: './jobs.component.html',
  styleUrls: ['./jobs.component.scss']
})
export class JobsComponent {

  public url: SafeResourceUrl;
  private router: Router;

  constructor(
    private sanitizer: DomSanitizer,
  ) {
    this.url = this.sanitizer.bypassSecurityTrustResourceUrl(environment.ONLINE_JOBS);
    this.router = ServiceInjector.injector.get(Router);
  }

  public goBack() {
    return this.router.navigate(['/support']);
  }
}

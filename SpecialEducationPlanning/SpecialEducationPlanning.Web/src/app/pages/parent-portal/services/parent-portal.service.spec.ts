import { TestBed } from '@angular/core/testing';

import { ParentPortalService } from './parent-portal.service';

describe('ParentPortalService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [ParentPortalService]
  }));

  it('should be created', () => {
    const service: ParentPortalService = TestBed.get(ParentPortalService);
    expect(service).toBeTruthy();
  });
});

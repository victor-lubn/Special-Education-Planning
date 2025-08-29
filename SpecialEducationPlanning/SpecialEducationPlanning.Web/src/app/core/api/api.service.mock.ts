import { PlanServiceMock } from './plan/plan.service.mock';
import { BuilderServiceMock } from './builder/builder.service.mock';
import { PostcodeServiceMock } from './postcode/postcode.service.mock';

export class ApiServiceMock {

  get plans(): PlanServiceMock {
    return new PlanServiceMock();
  }

  get builders(): BuilderServiceMock {
    return new BuilderServiceMock();
  }

  get postcodes(): PostcodeServiceMock {
    return new PostcodeServiceMock();
  }

}

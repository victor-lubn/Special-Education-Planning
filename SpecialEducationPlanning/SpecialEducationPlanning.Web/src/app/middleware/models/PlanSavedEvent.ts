import { ThreeDCEvent } from './ThreeDCEvent';
import { CatalogueEnum } from './CatalogueEnum';

export class PlanSavedEvent extends ThreeDCEvent {
  planId3DC: string;
  version3DC: string;
  EducationViewPlanUniqueId: string;
  thumbnailUrl: string;
  renderRequestJsonUrl: string;
  Catalogue: string;
}


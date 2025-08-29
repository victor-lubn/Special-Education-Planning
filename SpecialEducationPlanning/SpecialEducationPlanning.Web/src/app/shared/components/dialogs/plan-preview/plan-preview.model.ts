
import { Plan } from "../../../models/plan";
import { Version } from "../../../models/version";

export interface PlanPreviewComponentData {
  versionId: number,
  plan: Plan,
  showPromoteMaster?: boolean,
  isMasterVersion?: boolean,
  planVersions?: Version[],
  showButton?: boolean
}

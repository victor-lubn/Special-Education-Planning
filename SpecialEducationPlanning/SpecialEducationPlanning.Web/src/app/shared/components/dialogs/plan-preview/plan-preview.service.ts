import { PlanPreviewComponent } from "./plan-preview.component";
import { DialogsService } from "../../../../core/services/dialogs/dialogs.service";
import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { PlanPreviewComponentData } from "./plan-preview.model";

@Injectable({
  providedIn: 'root'
})
export class PlanPreviewService extends DialogsService {

  constructor(
    public matDialog: MatDialog
  ) {
    super(matDialog);
  }

  public planPreview(planPreviewData: PlanPreviewComponentData): Promise<boolean> {
    return this.openDialog<PlanPreviewComponent, boolean>(PlanPreviewComponent, {
      data: {
        versionId: planPreviewData.versionId,
        plan: planPreviewData.plan,
        showPromoteMaster: planPreviewData.showPromoteMaster,
        isMasterVersion: planPreviewData.isMasterVersion,
        planVersions: planPreviewData.planVersions,
        showButton: planPreviewData.showButton
      }
    });
  }
}

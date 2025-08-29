import { AssignPlanDialogComponent } from "./assign-plan-dialog.component";
import { DialogsService } from "../../../../../core/services/dialogs/dialogs.service";
import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { TableRecords } from "../../table/table.types";
import { Plan } from "../../../../models/plan";

@Injectable({
  providedIn: 'root'
})
export class AssignPlanDialogService extends DialogsService {

  constructor(
    public matDialog: MatDialog
  ) {
    super(matDialog);
  }

  public unassignedPlans(tablePlans: TableRecords<Plan>): Promise<Plan> {
    return this.openDialog<AssignPlanDialogComponent, Plan>(
      AssignPlanDialogComponent,
      {
        data: {
          tablePlans: tablePlans
        }
      });
  }
}
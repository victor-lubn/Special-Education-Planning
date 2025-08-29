import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { Plan } from '../../../shared/models/plan';
import { EducationToolType, PlanStateEnum } from '../../../shared/models/app-enums';
import { Comment } from '../../../shared/models/comment';
import { Action } from '../../../shared/models/action';
import { SortDescriptor } from '../../services/url-parser/sort-descriptor.model';
import { Version } from '../../../shared/models/version';
import { Catalog } from '../../../shared/models/catalog.model';
import { RomFile } from '../../../shared/models/app-files.model';
import { EndUser } from '../../../shared/models/end-user';
import { Soundtrack } from '../../../shared/models/soundtrack.model';
import { PlanTypeOption } from '../../../shared/models/plan-type.model';
import { UserInfoService } from '../../services/user-info/user-info.service';
import { Project } from '../../../shared/models/project';
import { Title } from '../../../shared/models/title.model';
import { TenderPackPlanPayload } from '../../..//shared/models/tenderPack-plan-payload';

@Injectable()
export class PlanService {

  constructor(
    private http: HttpClient,
    private userInfo: UserInfoService
  ) { }

  public getPlan(planId: number): Observable<Plan> {
    return this.http.get<Plan>(`/Plan/${planId}`);
  }

  // public createPlan(planObject: Plan): Observable<Plan> {
  //   return this.http.post<Plan>(`/Plan`, planObject);
  // }

  public updatePlan(planObject: Plan): Observable<Plan> {
    return this.http.put<Plan>(`/Plan`, planObject);
  }

  public updateTenderPackPlan(tenderPackPlanPayload: TenderPackPlanPayload): Observable<Plan> {
    return this.http.post<Plan>('/Plan/UpdateTenderPackPlan', tenderPackPlanPayload);
  }

  public generatePlanCode(): Observable<string> {
    return this.http.get<string>(`/Plan/GeneratePlanCode`);
  }

  public checkPlanName(planName: string) : Observable<boolean> {
    return this.http.get<boolean>(`/Plan/DuplicatedPlan?planName=${encodeURIComponent(planName)}`);
  }

  /** Returns an in-memmory javascript supported File */
  public getVersionFile(versionId: number): Observable<RomFile> {
    return this.http.get(`/Version/${versionId}/Rom`, { observe: 'response', responseType: 'arraybuffer' })
      .pipe(map((response) => {
        const contentDisposition = response.headers.get('Content-Disposition');
        return {
          type: response.headers.get('Content-Type'),
          fileName: contentDisposition ? contentDisposition.split('attachment; filename=')[1].split('; filename*')[0] : 'noFileName.Rom',
          romByteArray: new Uint8Array(response.body)
        };
      }));
  }

  /** Return directly the ArrayBuffer */
  // public getVersionFile(versionId: number): Observable<ArrayBuffer> {
  //   return this.http.get(`/Version/${versionId}/Rom`, { responseType: 'arraybuffer' });
  // }

  public saveVersion(planId: number, formData: FormData): Observable<any> {
    return this.http.post(`/Version/${planId}/SaveVersion`, formData);
  }

  public getVersionPreview(versionId: number): Observable<ArrayBuffer> {
    return this.http.get(`/Version/${versionId}/Preview`, { responseType: 'arraybuffer' });
  }

  public getNewPlanId(): Observable<any> {
    return this.http.get(`/Plan/PlanId`);
  }

  public getPlansFiltered(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Plan>> {
    return this.http.post<EnvelopeResponse<Plan>>(`/Plan/GetPlansFiltered`, pageDescriptor)
      .pipe(map((response) => {
        return {
          ...response,
          data: response.data.map((plan: Plan) => {
            return {
              ...plan,
              createdDate: plan.createdDate ? new Date(plan.createdDate) : null,
              updatedDate: plan.updatedDate ? new Date(plan.updatedDate) : null,
              lastOpen: plan.lastOpen ? new Date(plan.lastOpen) : null
            };
          })
        };
      }));
  }

  public getPlansForProject(projectId: string | number): Observable<Plan[]> {
    return this.http.get<Plan[]>(`/Plan/GetPlansForProject/${projectId}`);
  }

  public createSinglePlan(planObject: Plan): Observable<Plan> {
    return this.http.post<Plan>(`/Plan/PostSinglePlan`, planObject);
  }

  public createSingleTenderPlan(planObject: Plan): Observable<Plan> {
    return this.http.post<Plan>(`/Plan/PostSingleTenderPackPlan`, planObject);
  }

  public changePlanState(planId: number, newPlanState: PlanStateEnum): Observable<Plan> {
    return this.http.post<Plan>(`/Plan/ChangePlanState/${planId}`, newPlanState);
  }

  public getPlanActions(planId: number): Observable<Action[]> {
    return this.http.get<Action[]>(`/Plan/${planId}/PlanActions`)
      .pipe(map((response) => {
        return response.map((action) => {
          return {
            ...action,
            // Note for monday: I had to do this because dates came from backend as strings
            date: action.date ? new Date(action.date) : null
          };
        });
      }));
  }

  public getPlanVersions(planId: number): Observable<Version[]> {
    return this.http.get<Version[]>(`/Plan/${planId}/Version`)
      .pipe(map((response) => {
        return response.map((version) => {
          return {
            ...version,
            updatedDate: version.updatedDate ? new Date(version.updatedDate) : null
          };
        });
      }));
  }

  public getVersionById(versionId: number): Observable<Version> {
    return this.http.get<Version>(`/Version/${versionId}`);
  }

  public newPlanVersion(version: any): Observable<Version> {
    return this.http.post<Version>(`/Version`, version);
  }

  public createFirstPlanVersion(planId: number, catalogId: number, version3DC: number): Observable<Version> {
    return this.newPlanVersion({
      lastKnown3DCVersion: version3DC,
      PlanId: planId,
      VersionNumber: 0,
      CatalogId: catalogId
    })
  }

  public updateVersion(versionId: number, versionModel: any): Observable<any> {
    return this.http.put<any>(`/Version/${versionId}`, versionModel);
  }

  public getPlanComments(planId: number): Observable<Comment[]> {
    if (this.userInfo.hasPermission('Plan_Comment')) {
      return this.http.get<Comment[]>(`/Plan/${planId}/Comments`)
        .pipe(map((response) => {
          return response.map((comment) => {
            return {
              ...comment,
              date: comment.updatedDate ? new Date(comment.updatedDate) : null,
              updatedDate: comment.updatedDate ? new Date(comment.updatedDate) : null
            };
          });
        }));
    } else {
      return of([]);
    }
  }

  public postPlanComment(planId: number, comment: Comment): Observable<Comment> {
    return this.http.post<Comment>(`/Plan/${planId}/Comments`, comment)
      .pipe(map((response) => {
        return {
          ...response,
          date: response.updatedDate ? new Date(response.updatedDate) : null,
          updatedDate: response.updatedDate ? new Date(response.updatedDate) : null
        };
      }));
  }

  public assignBuilderToPlan(planId: number, builderId: number): Observable<Plan> {
    return this.http.post<Plan>(`/Plan/AssignBuilderToPlan?planId=${planId}&builderId=${builderId}`, {})
      .pipe(map((plan: Plan) => {
        return {
          ...plan,
          createdDate: plan.createdDate ? new Date(plan.createdDate) : null,
          updatedDate: plan.updatedDate ? new Date(plan.updatedDate) : null,
          lastOpen: plan.lastOpen ? new Date(plan.lastOpen) : null
        };
      }));
  }

  /** TODO Improve with proper KOA Page Descriptor */
  public getPlansSorted(builderId: number, sortModel: SortDescriptor): Observable<EnvelopeResponse<Plan>> {
    return this.http.post<EnvelopeResponse<Plan>>(`/Plan/GetPlansSorted?builderId=${builderId}`, sortModel)
      .pipe(map((response) => {
        return {
          ...response,
          data: response.data.map((plan: Plan) => {
            return {
              ...plan,
              createdDate: plan.createdDate ? new Date(plan.createdDate) : null,
              updatedDate: plan.updatedDate ? new Date(plan.updatedDate) : null,
              lastOpen: plan.lastOpen ? new Date(plan.lastOpen) : null
            };
          })
        };
      }));
  }

  public deleteComment(commentId: number): Observable<void> {
    return this.http.delete<void>(`/Comment/${commentId}`);
  }

  public deleteVersion(version: number): Observable<void> {
    return this.http.delete<void>(`/version/${version}`);
  }

  public editComment(body: any): Observable<Comment> {
    return this.http.put<Comment>(`/Comment/${body.id}`, body)
      .pipe(map((response) => {
        return {
          ...response,
          date: response.updatedDate ? new Date(response.updatedDate) : null,
          updatedDate: response.updatedDate ? new Date(response.updatedDate) : null
        };
      }));
  }

  public getCatalogs(EducationOrigin?: EducationToolType): Observable<Catalog[]> {
    return this.http.get<Catalog[]>(`/Catalog${EducationOrigin ? '?EducationOrigin=' + EducationOrigin : ''}`);
  }

  public getCatalogByNameAndEducationOrigin(catalogName: string, EducationOrigin: EducationToolType): Observable<Catalog> {
    return this.http.get<Catalog>(`/Catalog/${catalogName}/GetCode?EducationOrigin=${EducationOrigin}`);
  }

  public getPlanTypes(): Observable<PlanTypeOption[]> {
    return this.http.get<PlanTypeOption[]>(`/Plan/GetAllPlanTypes`);
  }

  public getMusicOptions(): Observable<Soundtrack[]> {
    return this.http.get<Soundtrack[]>('/Soundtrack');
  }

  public getCatalogById(catalogId: number) {
    return this.http.get<Catalog>(`/Catalog/${catalogId}`);
  }

  public getEndUserTitles(): Observable<Title[]> {
    return this.http.get<Title[]>(`/Title`);
  }

  public getEndUser(endUserId: number): Observable<EndUser> {
    return this.http.get<EndUser>(`/EndUser/${endUserId}`);
  }

  public validateExistingEndUsers(endUserObject: EndUser): Observable<any> {
    return this.http.post<any>(`/EndUser/SearchExistingEndUser`, endUserObject);
  }

  public updateEndUser(endUserObject: EndUser): Observable<EndUser> {
    return this.http.put<EndUser>(`/EndUser/${endUserObject.id}`, endUserObject);
  }

  public transferMultiplePlans(builderId: number, AiepId: number): Observable<void> {
    return this.http.post<void>(`/Plan/${builderId}/TransferMultiplePlanBetweenAieps/${AiepId}`, {});
  }

  public transferSinglePlan(planId: number, AiepId: number): Observable<void> {
    return this.http.post<void>(`/Plan/${planId}/TransferSinglePlanBetweenAieps/${AiepId}`, {});
  }

  public transferProjectPlansToAiep(projectId: number, AiepCode: string): Observable<void> {
    return this.http.post<void>(`/Plan/${projectId}/TransferProjectPlansToAiep/${AiepCode}`, {});
  }

  public unassignBuilderFromPlan(planId: number): Observable<void> {
    return this.http.post<void>(`/Plan/${planId}/UnassignBuilder`, {});
  }

  public getProject(projectId: number): Observable<Project> {
    return this.http.get<Project>(`/Project/${projectId}`);
  }

  public getArchivedPlans(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Plan>> {
    return this.http.post<EnvelopeResponse<Plan>>(`/Plan/GetAllArchivedPlans`, pageDescriptor);
  }

  public modifyVersionNotes(versionId: number, versionNotes: string): Observable<Version> {
    return this.http.put<Version>(`/Version/${versionId}/ModifyVersionNotes`,
      JSON.stringify(versionNotes),
      {
        headers: new HttpHeaders({
          'Content-Type': 'application/json'
        })
      });
  }

  public modifyVersionNotesAndQuote(versionId: number, versionNotes: string, quoteOrderNumber: string): Observable<Version> {
    return this.http.put<Version>(`/Version/${versionId}/ModifyVersionNotesAndQuote`,
      JSON.stringify({
        versionNotes: versionNotes,
        quoteOrderNumber: quoteOrderNumber
      }),
      {
        headers: new HttpHeaders({
          'Content-Type': 'application/json'
        })
      });
  }

  downloadFittersPackPdf(id) {
    return this.http.get(`/version/${id}/FittersPackPdf`, { responseType: 'arraybuffer' });
  }
}



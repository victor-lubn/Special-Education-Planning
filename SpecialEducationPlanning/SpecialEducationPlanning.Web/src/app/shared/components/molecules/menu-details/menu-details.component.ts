import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { UserInformation } from "../../../models/user-information";
import { iconNames } from "../../atoms/icon/icon.component";
import { SidebarService } from "../../sidebar/sidebar.service";

@Component({
  selector: "tdp-menu-details",
  templateUrl: "./menu-details.component.html",
  styleUrls: ["./menu-details.component.scss"],
})
export class MenuDetailsComponent implements OnInit {
  //icons
  public iconOptionsSize: string;
  public iconRelaseNotesDocument: string;
  public iconReleaseNotesWeb: string;
  public iconAbout: string;
  public iconQuestion: string;
  public iconShowProjects: string;

  @Input()
  public userInfo: UserInformation;

  @Input()
  public showSupport: boolean;

  @Output()
  public onLogout = new EventEmitter<void>();

  @Output()
  public onReleaseNotesDocument = new EventEmitter<void>();

  @Output()
  public onReleaseNotesWeb = new EventEmitter<void>();

  @Output()
  public onSupportDashboard = new EventEmitter<void>();

  @Output()
  public onAbout = new EventEmitter<void>();

  @Output()
  public onShowProjects = new EventEmitter<void>();

  constructor(private sidebarService: SidebarService) {
    this.iconOptionsSize = "36px";
    this.iconRelaseNotesDocument = iconNames.size36px.NOTES;
    this.iconReleaseNotesWeb = iconNames.size36px.WEBNOTES;
    this.iconAbout = iconNames.size36px.TEAM;
    this.iconQuestion = iconNames.size36px.QUESTIONS;
    this.iconShowProjects = iconNames.size36px.PROJECTS;
  }

  ngOnInit(): void {}

  public handleReleaseNotesDocument() {
    this.onReleaseNotesDocument.emit();
    this.closeMenuDetails();
  }

  public handleReleaseNotesWeb() {
    this.onReleaseNotesWeb.emit();
    this.closeMenuDetails();
  }

  public handleSupportDashboard() {
    this.onSupportDashboard.emit();
    this.closeMenuDetails();
  }

  public handleAbout() {
    this.onAbout.emit();
    this.closeMenuDetails();
  }

  public handleLogout() {
    this.onLogout.emit();
    this.closeMenuDetails();
  }

  public handleShowProjects() {
    this.onShowProjects.emit();
    this.closeMenuDetails();
  }

  public closeMenuDetails() {
    this.sidebarService.getSidebar("rightSidebar").close();
  }
}

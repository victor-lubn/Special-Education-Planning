import { Component, Input, OnChanges, OnInit } from "@angular/core";

export const iconNames = {
  size18px: {
    CLOSE: "Close.svg",
  },
  size24px: {
    TRASH_CAN_WHITE: "trash_can_24px_white.svg",
    PENCIL_WHITE: "pencil_white_24px.svg",
    EMAIL_BLACK: "email_24px_black.svg",
    MOBILE_BLACK: "mobile_24px_black.svg",
    TELEPHONE_BLACK: "telephone_24px_black.svg",
    BLACK_BACK_ARROW: "Black_Back_Arrow.svg",
    CLOSE: "close_24px_silver.svg",
    ACCOUNT_CIRCLE: "account_circle_24px_rounded.svg",
    ADD: "add_24px_rounded.svg",
    ADD_COMMENT: "add_comment_24px_rounded.svg",
    ANNOUNCENEMT: "announcement_black_24dp 1.svg",
    ARCHIVE: "archive_black_24dp (1) 1.svg",
    ARROW_DROPDOWN: "arrow_drop_down_black_24dp 1.svg",
    AUDIO: "Audio.svg",
    BACK_ARROW: "Back Arrow.svg",
    BADGE: "badge_black_24dp 1.svg",
    CALL: "call_24px_rounded.svg",
    CHECKBOX: "check_box_24px_rounded.svg",
    CHECKBOX_OUTLINE: "check_box_outline_blank_24px_rounded.svg",
    CHEVRON_LEFT: "Chevron-Left.svg",
    CHEVRON_RIGHT: "Chevron-Right.svg",
    CLEAR: "clear_24px_rounded.svg",
    DELETE: "delete_black_24dp 1.svg",
    DOWNLOAD: "Download.svg",
    DRAG: "drag_handle_black_24dp 1.svg",
    DVR: "dvr_black_24dp 1.svg",
    EXPAND: "expand_more_24px_rounded.svg",
    FAVORITE: "favorite_black_24dp 1.svg",
    FAVORITE_BORDER: "favorite_border_black_24dp 1.svg",
    FOLDER: "folder_24px_rounded.svg",
    FORMAT_BOLD: "format_bold_24px_rounded.svg",
    FORMAT_COLOR: "format_color_text_24px_rounded.svg",
    FORMAT_ITALIC: "format_italicon_rounded.svg",
    GROUPS: "groups_black_24dp (1) 3.svg",
    INFO: "info_black_24dp 1.svg",
    INSERT_COMMENT: "insert_comment_24px_rounded.svg",
    INSIGHTS: "insights_black_24dp 2.svg",
    LANGUAGES: "Languages.svg",
    LAUNCH_BACK: "launch_black_24dp 1.svg",
    LIST_ALL: "list_alt_black_24dp 2.svg",
    LOOP: "loop_24px_rounded.svg",
    MAIL: "mail_24px_rounded.svg",
    MODE: "mode_24px_rounded.svg",
    MORE_VERTICAL: "more_vert_24px_rounded.svg",
    NEW_RELEASES: "new_releases_black_24dp 2.svg",
    NOTIFICATIONS: "notifications_black_24dp 1.svg",
    OFFLINE: "offline-icon.component.svg",
    OPEN_IN_FULL: "open_in_full_black_24dp 1.svg",
    PAGINATION_END: "Pagination-End.svg",
    PAGINATION_START: "Pagination-Start.svg",
    PERSON: "person_24px_rounded.svg",
    PREVIEW: "preview_black_24dp 1.svg",
    QUESTION: "Question.svg",
    RADIO_BUTTON_CHECKED: "radio_button_checked_24px_rounded.svg",
    RADIO_BUTTON_UNCHECKED: "radio_button_unchecked_24px_rounded.svg",
    REMOVE_CIRCLE: "remove_circle_black_24dp 1.svg",
    RESTORE_PAGE: "restore_page_black_24dp 1.svg",
    SEARCH: "search-black-36dp.svg",
    STAR: "star_black_24dp (1) 1.svg",
    STAR_BORDER: "star_border_black_24dp 1.svg",
    STAY_CURRENT_PORTRAIT: "stay_current_portrait_24px_rounded.svg",
    STORE: "store_24px_rounded.svg",
    TEXT_FIELDS: "text_fields_24px_rounded.svg",
    UNFOLD_LESS: "unfold_less_black_24dp 1.svg",
    UNFOLD_MORE: "unfold_more_black_24dp 1.svg",
    UPLOAD: "Upload.svg",
    VISIBILITY: "visibility_24px_rounded.svg",
    WORK: "work_24px_rounded.svg",
    CHECK_WHITE: "checkOk.svg",
    CANCEL_WHITE: "cancel_white.svg",
    OFFLINE_INFORMATIVE: "offline_informative.svg",
    OFFLINE_BLACK: "offline_black.svg",
  },
  size36px: {
    SCREEN_SILVER: "screen_silver_36px.svg",
    ACCOUNT_CIRCLE_BLACK: "account_circle_36px_black.svg",
    ACCOUNT_CIRCLE: "account_circle_36px_rounded.svg",
    ADMIN_PANEL: "admin_panel_settings_black_36dp (1) 1.svg",
    ANNOUNCEMENT: "announcement_black_36dp 2.svg",
    APPS_ROUNDED: "apps_rounded.svg",
    ARROW_DROPDOWN: "arrow_drop_down_rounded.svg",
    BADGE: "badge_black_36dp 2.svg",
    CALL: "call_36px_rounded.svg",
    CAMPAIGN: "Campaign.svg",
    CHECKBOX: "check_box_36px.svg",
    CHECKBOX_OUTLINE: "check_box_outline_blank_.svg",
    CLEAR: "clear_36px.svg",
    DVR: "dvr_black_24dp 2.svg",
    EXPAND: "expand_more_36px_rounded.svg",
    HELP: "help_rounded.svg",
    INSERT_DRIVE: "insert_drive_file_24px_rounded.svg",
    LOOP: "loop.svg",
    MAIL: "mail_36px.svg",
    MODE: "mode_24px_rounded.svg",
    NOTES: "Notes.svg",
    QUESTIONS: "Questions_rounded.svg",
    SEARCH: "search-black-36dp 2.svg",
    STORE: "store_36px.svg",
    TEAM: "Team_rounded.svg",
    WEBNOTES: "web notes_rounded.svg",
    PROJECTS: "nine-squares.svg",
  },
  size48px: {
    ANGLE: "angle.svg",
    CLOUD_UPLOAD: "cloud_upload_48px.svg",
    GROUPS: "groups_black_24dp (1) 2.svg",
    PREVIEW_UNAVAILABLE: "Image_unavailable.svg",
  },
  size100px: {
    DVR: "dvr_black_24dp 2.svg",
    GROUPS: "groups_black_24dp (1) 2.svg",
    HELP: "help.svg",
    INSIGHTS: "insights_black_24dp 1.svg",
    LIST_ALL: "list_alt_black_24dp 1.svg",
    NEW_RELEASES: "new_releases_black_24dp 1.svg",
    STORE: "store_24px.svg",
  },
  size146px: {
    CHECK_CIRCLE: "check_circle_24px_rounded.svg",
    GROUPS: "groups_black_24dp (1) 1.svg",
  },
};

@Component({
  selector: "tdp-icon",
  templateUrl: "./icon.component.html",
  styleUrls: ["./icon.component.scss"],
})
export class IconComponent implements OnInit, OnChanges {
  path = "/assets/icons/app-icons";
  source: string;

  @Input()
  size: "18px" | "24px" | "36px" | "48px" | "100px" | "146px";

  @Input()
  name: string;

  constructor() {}

  ngOnInit(): void {
    this.setImageSource();
  }

  ngOnChanges(): void {
    this.setImageSource();
  }

  setImageSource(): void {
    this.source = `${this.path}/${this.size}/${this.name}`;
  }
}

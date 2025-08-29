namespace SpecialEducationPlanning
.Domain.Enum
{
    public enum PermissionType
    {

        Report_Request, // Get data from TDP to generate reports

        Log_Request, //Read the system logs

        Structure_Management, //Admin actions on: Country, Region, Area, Aiep and Educationer

        User_Management, // Admin actions on: User

        Role_Management, // Admin actions on: Roles and permissions

        Data_Management, //Admin actions on:Release Notes, SoundTracks and Catalogs

        Plan_Management, //Admin actions on: Plan, Version, Builder, Comment, RomItem and Enduser

        Plan_Create, //Create new:Plan, Version, Builder, Comment, RomItem and Enduser

        Plan_Modify, //Modify existing: Plan, Version, Builder, Comment, RomItem and Enduser

        Plan_Delete, //Modify existing: Plan, Version, Builder, Comment, RomItem and Enduser

        Plan_Publish, //Request a publish from a Version or Plan(Master Version)

        Plan_Comment, //Add Comments on Plan Activity Timeline

        Comment_Management, //Manage actions on Comments on Plan Activity Timeline

        Hub_Management, //Access to all the Aiep and plans for transferring

        Project_Management, //Access to View Project button for displaying CHTP projects
    }
}


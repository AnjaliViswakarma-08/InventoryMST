namespace InventoryMS.Helpers;

public static class RoleName
{
    public const string Owner = "Owner";
    public const string HR = "HR";
    public const string AdminStaff = "AdminStaff";
    public const string ViewerStaff = "ViewerStaff";
    public const string EditorStaff = "EditorStaff";

    // Policy convenience groups
    public const string AllStaff = "AdminStaff,ViewerStaff,EditorStaff";
    public const string StaffWithWrite = "AdminStaff,EditorStaff";
    public const string StaffWithDelete = "AdminStaff";
}

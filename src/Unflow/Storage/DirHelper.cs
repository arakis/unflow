namespace Unflow;

public static class DirHelper
{
    public static readonly string RootDataDir;

    static DirHelper()
    {
        var userDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var unflowDataDir = Path.Combine(userDataDir, "unflow");
        Directory.CreateDirectory(unflowDataDir);
        RootDataDir = unflowDataDir;
    }

    public static string GetGroupDataDir(Group group)
    {
        var groupDataDir = Path.Combine(RootDataDir, "group", group.Name);
        Directory.CreateDirectory(groupDataDir);
        return groupDataDir;
    }
}
namespace JBpunch.Authorization;

public static class JBpunchPermissions
{
    public const string GroupName = "CheckIn";

    public const string ClockDataManage = GroupName + ".ClockData";
    public const string ClockDataCreate = ClockDataManage + ".Create";
    public const string ClockDataDelete = ClockDataManage + ".Delete";

    public const string GpsPuncheManage = GroupName + ".GpsPunche";
    public const string GpsPuncheCreate = GpsPuncheManage + ".Create";
    public const string GpsPuncheDelete = GpsPuncheManage + ".Delete";

    public static IEnumerable<string> GetAll()
    {
        yield return ClockDataManage;
        yield return ClockDataCreate;
        yield return ClockDataDelete;
        yield return GpsPuncheManage;
        yield return GpsPuncheCreate;
        yield return GpsPuncheDelete;
    }
}

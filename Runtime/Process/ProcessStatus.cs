namespace Calluna.Process
{
    public enum ProcessStatus
    {
        Pending = 1 << 0,
        Running = 1 << 1,
        Done = 1 << 2,
        Failed = 1 << 3,
        Aborted = 1 << 4
    }
}

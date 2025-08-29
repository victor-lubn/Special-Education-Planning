namespace SpecialEducationPlanning
.Domain.Enum
{
    public enum FittersPackStatusEnum
    {
        Queued = 1,
        Processing,
        Completed,
        Failed,
        OverDueQueued,
        RetryFailed,
        OverDueFailed
    }
}

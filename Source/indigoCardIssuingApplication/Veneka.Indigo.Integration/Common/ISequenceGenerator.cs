using System;

namespace Veneka.Indigo.Integration.Common
{    public enum ResetPeriod { NONE = 0, DAILY = 1, WEEKLY = 2, MONTHLY = 3, YEARLY = 4 };

    public interface ISequenceGenerator : IDisposable
    {
        int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod);
        long NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod);
    }
}
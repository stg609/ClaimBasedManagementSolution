using System;

namespace Common.Domain
{
    public class BaseEntity<TKey>
    {
        public TKey Key { get; set; }
    }

    public interface IAuditable
    {
        DateTime LastUpdatedTime { get; set; }
    }
}

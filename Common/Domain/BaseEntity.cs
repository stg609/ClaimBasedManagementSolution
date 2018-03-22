using System;

namespace Domain
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

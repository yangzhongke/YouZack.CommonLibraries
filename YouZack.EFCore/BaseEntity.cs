using System;

namespace Infrastructures.EFCore
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
    }
}

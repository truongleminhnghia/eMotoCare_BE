using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Base
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        protected BaseEntity()
        {
            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        protected void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

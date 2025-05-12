using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;

        //DateTime UtcNow { get; } dùng như này thì bên implement phải khai báo

    }
}

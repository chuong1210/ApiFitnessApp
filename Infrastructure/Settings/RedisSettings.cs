using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Settings
{
    public class RedisSettings
    {

        public string ConnectionString { get; set; } = string.Empty;
        public int OtpExpiryMinutes { get; set; } = 5;
    }
}

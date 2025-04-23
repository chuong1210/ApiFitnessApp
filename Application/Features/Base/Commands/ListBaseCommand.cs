using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Base.Commands
{
    public record ListBaseCommand
    {
        public string? Filters { get; set; }
        public string? Sorts { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 100;
        public bool IsAllDetail { get; set; }
       public string searchTerm { get; set; }
    }
}

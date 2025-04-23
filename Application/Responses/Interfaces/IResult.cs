using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses.Interfaces
{
    public interface IResult<T>
    {
        T Data { get; set; }

        List<string> Messages { get; set; }

        bool Succeeded { get; set; }

        int Code { get; set; }
    }
}

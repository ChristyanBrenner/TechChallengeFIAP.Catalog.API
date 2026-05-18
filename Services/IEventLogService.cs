using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEventLogService
    {
        Task RegistrarAsync(EventLog eventLog);
    }
}

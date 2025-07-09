using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKaraoke.Services
{
    public interface IDatabaseService
    {
        Task InitializeDatabaseAsync();
        Task<bool> IsDatabaseAvailableAsync();
        Task<bool> HasPendingMigrationsAsync();
    }
}

using GymManagementBLL.ViewModels.AnalyticsVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        AnalyticsViewModel GetAnalyticsData();
    }
}

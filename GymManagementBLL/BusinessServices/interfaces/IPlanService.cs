using GymManagementBLL.View_Models.Plan_VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.BusinessServices.interfaces
{
    public interface IPlanService
    {

        IEnumerable<PlanViewModel> GetAllPlans();

        PlanViewModel? GetPlanDetails(int PlanId);
        PlanToUpdateViewModel?  GetPlanToUpdate(int PlanId);

        bool UpdatePlan(int PlanId , PlanToUpdateViewModel planToUpdate);

        bool TogggleStatus(int PlanId);
    }
}

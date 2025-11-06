using AutoMapper;
using GymManagementBLL.BusinessServices.interfaces;
using GymManagementBLL.View_Models.Plan_VM;
using GymManagementDAL.Entities;
using GymManagementDAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.BusinessServices.implementaion
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService( IUnitOfWork unitOfWork , IMapper mapper)
        {
           _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<PlanViewModel> GetAllPlans()
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetAll();
            if (plan is null || !plan.Any()) return [];

            #region manual
            //return plan.Select(P => new PlanViewModel()
            //{
            //    Id = P.Id,
            //    Name = P.Name,
            //    Description = P.Description,
            //    DurationDays = P.DurationDays,
            //    Price = P.Price,
            //    IsActive = P.IsActive
            //}); 
            #endregion

            return _mapper.Map<IEnumerable<PlanViewModel>>(plan);
        }

        public PlanViewModel? GetPlanDetails(int PlanId)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(PlanId);
            if (plan is null ) return null;

            #region manual
            //return  new PlanViewModel()
            //{
            //    Id = plan.Id,
            //    Name = plan.Name,
            //    Description = plan.Description,
            //    DurationDays = plan.DurationDays,
            //    Price = plan.Price,
            //    IsActive = plan.IsActive
            //}; 
            #endregion

            return _mapper.Map<PlanViewModel>(plan);
        }

        public PlanToUpdateViewModel? GetPlanToUpdate(int PlanId)
        {
            var planRepo = _unitOfWork.GetRepository<Plan>();
            var plan = planRepo.GetById(PlanId);
            if (plan is null || plan.IsActive == false || HasActiveMemberships(PlanId))
                return null;
            #region manual mapp

            //return new PlanToUpdateViewModel()
            //{
            //    Name = plan.Name,
            //    Description = plan.Description,
            //    DurationDays = plan.DurationDays,
            //    Price = plan.Price
            //}; 
            #endregion
            return _mapper.Map<PlanToUpdateViewModel>(plan);

        }

        public bool UpdatePlan(int PlanId, PlanToUpdateViewModel planToUpdate)
        {
            var planRepo = _unitOfWork.GetRepository<Plan>();
            var plan = planRepo.GetById(PlanId);
            if (plan is null || planToUpdate is null)
                return false;

            #region manual

            //(plan.Description, plan.DurationDays, plan.Price) =
            //    (planToUpdate.Description, planToUpdate.DurationDays, planToUpdate.Price);
            //plan.UpdatedAt = DateTime.Now; 
            #endregion

             _mapper.Map(planToUpdate,plan);
            try
            {
                planRepo.Update(plan);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }


        }

        bool IPlanService.TogggleStatus(int PlanId)
        {
            var planRepo = _unitOfWork.GetRepository<Plan>();
            var plan = planRepo.GetById(PlanId);
            if (plan is null || HasActiveMemberships(PlanId))
                return false;
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            planRepo.Update(plan);
            return _unitOfWork.SaveChanges() > 0;
        }

        #region Helper Method

        private bool HasActiveMemberships(int planId)
        {
            var activeMemberships = _unitOfWork.GetRepository<Membership>()
                                               .GetAll(M =>M.PlanId == planId && M.Status == "Active");
            return activeMemberships.Any();
            
        }


        #endregion
    }
}

using AutoMapper;
using GymManagementBLL.BusinessServices.interfaces;
using GymManagementBLL.View_Models.Session_VM;
using GymManagementDAL.Entities;
using GymManagementDAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace GymManagementBLL.BusinessServices.implementaion
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool CreateSession(CreateSessionViewModel createSession)
        {
            try
            {
                if (!IsTrainerExit(createSession.TrainerId))
                    return false;

                if (!IsCategoryExit(createSession.CategoryId))
                    return false;
                if (!IsDateTimeValid(createSession.StartDate, createSession.EndDate))
                    return false;

                if (createSession.Capacity > 25 || createSession.Capacity < 0)
                    return false;

                var SessionToCreate = _mapper.Map<Session>(createSession);

                _unitOfWork.SessionRepository.Add(SessionToCreate);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public IEnumerable<SessionViewModel> GetAllSessions()
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = sessionRepo.GetAllWithCategoryAndTrainer();
            if (!sessions.Any())
                return [];
            #region Manula mapping
            //return sessions.Select(S => new SessionViewModel()
            //{
            //    Id = S.Id,
            //    Description = S.Description,
            //    StartDate = S.StartDate,
            //    EndDate = S.EndDate,
            //    Capacity = S.Capacity,
            //    TrainerName = S.Trainer.Name,
            //    CategoryName = S.Category.CategoryName,
            //    AvailableSlots = S.Capacity - sessionRepo.GetCountOfBookedSlots(S.Id)
            //}); 
            #endregion

            var mappedSessions = _mapper.Map<IEnumerable< Session>,IEnumerable< SessionViewModel>>(sessions);
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - sessionRepo.GetCountOfBookedSlots(session.Id);
                
            }
            return mappedSessions;
        }

        public SessionViewModel? GetSessionDetails(int sessionId)
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var session = sessionRepo.GetByIdWithTrainerAndCategory(sessionId);
            if (session is null)
                return null;
            #region manual mapping

            //return new SessionViewModel()
            //{
            //    Id = session.Id,
            //    Description = session.Description,
            //    StartDate = session.StartDate,
            //    EndDate = session.EndDate,
            //    Capacity = session.Capacity,
            //    TrainerName = session.Trainer.Name,
            //    CategoryName = session.Category.CategoryName,
            //    AvailableSlots = session.Capacity - sessionRepo.GetCountOfBookedSlots(session.Id)
            //};

            #endregion

            #region Auto mapper

            var mappedSession = _mapper.Map<Session,SessionViewModel>(session);
            mappedSession.AvailableSlots = session.Capacity - sessionRepo.GetCountOfBookedSlots(session.Id);
            return mappedSession;

            #endregion
        }


        public UpdateSessionViewModel? GetSessionToUpdate(int sessionId)
        {
            var session = _unitOfWork.SessionRepository.GetById(sessionId);
            if (IsSessionAvailableForUpdate(session !))
                return null;
              
            return _mapper.Map<UpdateSessionViewModel>(session);

        }

        public bool UpdateSession(int sessionId, UpdateSessionViewModel updateSession)
        {
            try
            {
                var session = _unitOfWork.SessionRepository.GetById(sessionId);

                if (!IsSessionAvailableForUpdate(session!))
                    return false;

                if (!IsTrainerExit(updateSession.TrainerId))
                    return false;

                if (!IsDateTimeValid(updateSession.StartDate, updateSession.EndDate))
                    return false;

                _mapper.Map(updateSession, session);
                _unitOfWork.SessionRepository.Update(session!);
                session!.UpdatedAt = DateTime.Now;
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }


        public bool DeleteSession(int sessionId)
        {
            try
            {
                var session = _unitOfWork.SessionRepository.GetById(sessionId);
                if (!IsSessionAvailableForRemoving(session!))
                    return false;

                _unitOfWork.SessionRepository.Delete(session!);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }




        #region HelperMethods

        private bool IsTrainerExit(int trainerId)
        {
           return _unitOfWork.GetRepository<Trainer>().GetById( trainerId) is not null;
        }
        private bool IsCategoryExit(int categoryId)
        {
            return _unitOfWork.GetRepository<Category>().GetById(categoryId) is not null;
        }
        private bool IsDateTimeValid(DateTime startDate , DateTime endDate)
        {
            return startDate < endDate;
        }


        private bool IsSessionAvailableForUpdate(Session session)
        {

            if (session == null) return false;

            if (session.EndDate < DateTime.Now) return false;

            if (session.StartDate <= DateTime.Now) return false;

            var hasActiveBooking = _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id) > 0;

            if (hasActiveBooking) return false;

            return true;
        }

        private bool IsSessionAvailableForRemoving(Session session)
        {

            if (session == null) return false;

         

            if (session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) 
                return false;
            if (session.StartDate > DateTime.Now) 
                return false;

            var hasActiveBooking = _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id) > 0;

            if (hasActiveBooking) return false;

            return true;
        }


        #endregion


    }
}

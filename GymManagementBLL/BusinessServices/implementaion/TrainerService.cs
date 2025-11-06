using AutoMapper;
using GymManagementBLL.BusinessServices.interfaces;
using GymManagementBLL.View_Models;
using GymManagementBLL.View_Models.Trainer_VM;
using GymManagementDAL.Entities;
using GymManagementDAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.BusinessServices.implementaion
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public bool CreateTrainer(CreateTrainerViewModel createTrainer)
        {
            if (createTrainer is null || IsEmailExit(createTrainer.Email) || IsPhoneExit(createTrainer.Phone))
                return false;
            #region manual mapp
            //var trainer = new Trainer()
            //{
            //    Name = createTrainer.Name,
            //    Email = createTrainer.Email,
            //    Phone = createTrainer.Phone,
            //    Gender = createTrainer.Gender,
            //    DateOfBirth = createTrainer.DateOfBirth,
            //    Address = new Address()
            //    {
            //        BuildingNumber = createTrainer.BuildingNumber,
            //        City = createTrainer.City,
            //        Street = createTrainer.Street,
            //    },
            //    Specialties = createTrainer.Specialties,
            //}; 
            #endregion

            var trainer = _mapper.Map<CreateTrainerViewModel ,Trainer>(createTrainer);
            try
            {
                _unitOfWork.GetRepository<Trainer>().Add(trainer);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool DeleteTrainer(int trainerId)
        {
            var trainerRepo = _unitOfWork.GetRepository<Trainer>();
            var trainer = trainerRepo.GetById(trainerId);
            if (trainer is null || HasFutureSessions(trainerId)) 
                return false;
            try
            {
                trainerRepo.Delete(trainer);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var trainers = _unitOfWork.GetRepository<Trainer>().GetAll();

            if (trainers is null ||! trainers.Any())
                return [];

            #region manual mapp
            //return trainers.Select(T => new TrainerViewModel
            //{
            //    Id = T.Id,
            //    Name = T.Name,
            //    Email = T.Email,
            //    Phone = T.Phone,
            //    Specialties = T.Specialties.ToString()

            //}); 
            #endregion

            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);

        }

        public TrainerViewModel? GetTrainerDetails(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
            if (trainer is null) return null;

            #region manual
            //return new TrainerViewModel()
            //{
            //    Name = trainer.Name,
            //    Email = trainer.Email,
            //    Phone = trainer.Phone,
            //    Gender = trainer.Gender.ToString(),
            //    DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
            //    Address = $"{trainer.Address.BuildingNumber}-{trainer.Address.City}-{trainer.Address.Street}",
            //   Specialties = trainer.Specialties.ToString(),
            //}; 
            #endregion

            return _mapper.Map<TrainerViewModel>(trainer);
        }

        public TrainerToUpdateViewModel? GetTrainerToUpdate(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
            if (trainer is null) return null;
            #region manual
            //return new TrainerToUpdateViewModel()
            //{
            //    Name = trainer.Name,
            //    Email = trainer.Email,
            //    Phone = trainer.Phone,
            //    Specialties = trainer.Specialties,
            //    BuildingNumber = trainer.Address.BuildingNumber,
            //    City = trainer.Address.City,
            //    Street = trainer.Address.Street,

            //}; 
            #endregion

            return _mapper.Map<TrainerToUpdateViewModel>(trainer);
        }

        public bool UpdateTrainer(int id, TrainerToUpdateViewModel trainerToUpdate)
        {
            var trainerRepo = _unitOfWork.GetRepository<Trainer>();

            var EmailExistFromAnotherExistTrainer = trainerRepo
                .GetAll(T => T.Email == trainerToUpdate.Email && T.Id != id)
                .Any();
            var PhoneExistFromAnotherExistTrainer = trainerRepo
                .GetAll(T => T.Phone == trainerToUpdate.Phone && T.Id != id)
                .Any();

            if (EmailExistFromAnotherExistTrainer || PhoneExistFromAnotherExistTrainer || trainerToUpdate is null)
                return false;

            var trainer = trainerRepo.GetById(id);
            if (trainer is null) return false;

            trainer.Email = trainerToUpdate.Email;
            trainer.Phone = trainerToUpdate.Phone;
            trainer.Address.BuildingNumber = trainerToUpdate.BuildingNumber;
            trainer.Address.City = trainerToUpdate.City;
            trainer.Address.Street = trainerToUpdate.Street;
            trainer.Specialties = trainerToUpdate.Specialties;
            trainer.UpdatedAt = DateTime.Now;


            try
            {
                trainerRepo.Update(trainer);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {

                return false;
            }

           
        }


        #region Helper Methods

        private bool IsEmailExit(string email)
        {
            return _unitOfWork.GetRepository<Trainer>().GetAll(E => E.Email == email).Any();
        }
        private bool IsPhoneExit(string phone)
        {
            return _unitOfWork.GetRepository<Trainer>().GetAll(E => E.Phone == phone).Any();
        }

        private bool HasFutureSessions(int trainerId)
        {
            return _unitOfWork.GetRepository<Session>()
                .GetAll(S=>S.TrainerId == trainerId && S.StartDate > DateTime.Now)
                .Any();
        }

        #endregion



    }
}

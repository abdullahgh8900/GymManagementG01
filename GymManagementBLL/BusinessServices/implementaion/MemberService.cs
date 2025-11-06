using AutoMapper;
using GymManagementBLL.BusinessServices.interfaces;
using GymManagementBLL.View_Models;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Implementaion;
using GymManagementDAL.Repositories.Interfaces;
using GymManagementDAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.BusinessServices.implementaion
{
    class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool CreateMember(CreateMemberViewModel createMember)
        {


            if (IsEmailExit(createMember.Email) || IsPhoneExit(createMember.Phone))
                return false;

            #region Manual
            //var member = new Member()
            //{
            //    Name = createMember.Name,
            //    Email = createMember.Email,
            //    Phone = createMember.Phone,
            //    Gender = createMember.Gender,
            //    DateOfBirth = createMember.DateOfBirth,
            //    Address = new Address()
            //    {
            //        BuildingNumber = createMember.BuildingNumber,
            //        City = createMember.City,
            //        Street = createMember.Street,
            //    },
            //    HealthRecord = new HealthRecord()
            //    {
            //        Height = createMember.HealthRecord.Height,
            //        Weight = createMember.HealthRecord.Weight,
            //        BloodType = createMember.HealthRecord.BloodType,
            //        Note = createMember.HealthRecord.Note,

            //    }

            //}; 
            #endregion

            var member = _mapper.Map<CreateMemberViewModel, Member>(createMember);
            _unitOfWork.GetRepository<Member>().Add(member);

            return _unitOfWork.SaveChanges() > 0;
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll();
            if (members is null || !members.Any()) return [];

            #region Manula Mapping First Way

            //var listOfMembersViewModel = new List<MemberViewModel>();
            //foreach (var member in members)
            //{
            //    var memberViewModel = new MemberViewModel()
            //    {
            //        Id = member.Id,
            //        Name = member.Name,
            //        Phone = member.Phone,
            //        Photo = member.Photo,
            //        Email = member.Email,
            //        Gender = member.Gender.ToString()
            //    }
            //    ;
            //    listOfMembersViewModel.Add(memberViewModel);
            //}
            //return listOfMembersViewModel; 

            #endregion

            #region Manual Mapping Second Way 

            //var membersViewModel = members.Select(M => new MemberViewModel
            //{
            //    Id = M.Id,
            //    Name = M.Name,
            //    Phone = M.Phone,
            //    Photo = M.Photo,
            //    Email = M.Email,
            //    Gender = M.Gender.ToString()

            //});
            #endregion

            return  _mapper.Map<IEnumerable<Member> ,IEnumerable<MemberViewModel>>(members);
          
        }


        public MemberViewModel? GetMemberDetails(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null) return null;
            #region manual

            //var MemberViewModel = new MemberViewModel()
            //{
            //    Name = member.Name,
            //    Email = member.Email,
            //    Phone = member.Phone,
            //    Gender = member.Gender.ToString(),
            //    DateOfBirth = member.DateOfBirth.ToShortDateString(),
            //    Address = $"{member.Address.BuildingNumber}-{member.Address.City}-{member.Address.Street}",
            //    Photo = member.Photo,
            //}; 
            #endregion

            var MemberViewModel = _mapper.Map<MemberViewModel>(member);
            var memership = _unitOfWork.GetRepository<Membership>()
                            .GetAll(M => M.MemberId == member.Id && M.Status == "Active")
                            .FirstOrDefault();
            if (memership is null) return null;

            MemberViewModel.MembershipStartDate = memership.CreatedAt.ToShortDateString();
            MemberViewModel.MembershipEndDate = memership.EndDate.ToShortDateString();
            var plan = _unitOfWork.GetRepository<Plan>().GetById(memership.PlanId);
            if (plan is null) return null;

            MemberViewModel.PlanName = plan.Name;

            return MemberViewModel;


        }


        public HealthRecordViewModel? GetMemberHealthDetails(int memberId)
        {
            var memberHealtRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(memberId);
            if (memberHealtRecord is null) return null;
            #region manual map
            //return new HealthRecordViewModel()
            //{
            //    Weight = memberHealtRecord.Weight,
            //    Height = memberHealtRecord.Height,
            //    BloodType = memberHealtRecord.BloodType,
            //    Note = memberHealtRecord.Note

            //}; 
            #endregion

            return _mapper.Map<HealthRecordViewModel>(memberHealtRecord);
        }

        public MemberToUpdateViewModel? GetMemberDetailsToUpdate(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null) return null;
            #region manual mapp
            //return new MemberToUpdateViewModel()
            //{
            //    Name = member.Name,
            //    Email = member.Email,
            //    Phone = member.Phone,
            //    Photo = member.Photo,
            //    BuildingNumber = member.Address.BuildingNumber,
            //    City = member.Address.City,
            //    Street = member.Address.Street
            //}; 
            #endregion

            return _mapper.Map<MemberToUpdateViewModel>(member);

        }

        public bool UpdateMember(int memberId, MemberToUpdateViewModel memberToUpdate)
        {

            try
            {
                if (IsEmailExit(memberToUpdate.Email) || IsPhoneExit(memberToUpdate.Phone))
                    return false;

                var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
                if (member is null) return false;



                member.Email = memberToUpdate.Email;
                member.Phone = memberToUpdate.Phone;
                member.Address.BuildingNumber = memberToUpdate.BuildingNumber;
                member.Address.City = memberToUpdate.City;
                member.Address.Street = memberToUpdate.Street;
                member.UpdatedAt = DateTime.Now;




                _unitOfWork.GetRepository<Member>().Update(member);
                return _unitOfWork.SaveChanges() > 0;

            }
            catch (Exception e)
            {

                return false;
            }

        }


        public bool RemoveMember(int memberId)
        {
            try
            {
                var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
                if (member is null) return false;

                var memberSessionIds = _unitOfWork.GetRepository<MemberSession>()
                                    .GetAll(M => M.MemberId == memberId)
                                    .Select(M => M.SessionId);

                var hasFutureSession = _unitOfWork.GetRepository<Session>()
                                      .GetAll(S => memberSessionIds.Contains(S.Id) && S.StartDate > DateTime.Now)
                                      .Any();

                if (hasFutureSession) return false;

                var memberShips = _unitOfWork.GetRepository<Membership>().GetAll(M => M.MemberId == memberId);

                if (memberShips.Any())
                {
                    foreach (var memberShip in memberShips)
                    {
                        _unitOfWork.GetRepository<Membership>().Delete(memberShip);
                    }
                }

                _unitOfWork.GetRepository<Member>().Delete(member);
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
            return _unitOfWork.GetRepository<Member>().GetAll(E => E.Email == email).Any();
        }
        private bool IsPhoneExit(string phone)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(E => E.Phone == phone).Any();
        }

        #endregion
    }
}

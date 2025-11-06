using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Entities
{
    public class Member:GymUser
    {
        //joinDate = CreatedAt

        public string ? Photo { get; set; }

        #region Relationships

        #region Member has Health Record
        public HealthRecord HealthRecord { get; set; }

        #endregion

        #region Member Has Membership
        public ICollection<Membership> Memberships { get; set; }
        #endregion

        #region Member - Sessions 
        public ICollection<MemberSession> Sessions { get; set; }
        #endregion

        #endregion
    }
}

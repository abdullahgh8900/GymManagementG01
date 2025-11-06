using GymManagementBLL.View_Models.Session_VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.BusinessServices.interfaces
{
    public interface ISessionService
    {
        IEnumerable<SessionViewModel> GetAllSessions();

        SessionViewModel? GetSessionDetails(int sessionId);

        bool CreateSession(CreateSessionViewModel createSession);

        UpdateSessionViewModel? GetSessionToUpdate(int sessionId);

        bool UpdateSession(int sessionId, UpdateSessionViewModel updateSession);

        bool DeleteSession(int sessionId);
    }
}

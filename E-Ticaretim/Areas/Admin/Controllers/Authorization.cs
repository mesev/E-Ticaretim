namespace E_Ticaretim.Areas.Admin.Controllers
{
    public class Authorization
    {
        public bool IsAuthorized(string authorization,ISession session)
        {
            string? sessionAuthorization = session.GetString(authorization);
            if(sessionAuthorization == "True")
            {
                return true;
            }
            return false;
        }
    }
}

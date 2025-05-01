namespace SM_Web.Services
{
    public class BaseControllerService
    {
        public ApiService ApiService { get; }
        public JwtHelperService JwtHelper { get; }

        public BaseControllerService(ApiService apiService, JwtHelperService jwtHelper)
        {
            ApiService = apiService;
            JwtHelper = jwtHelper;
        }
    }
}

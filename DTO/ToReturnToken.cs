namespace UserGuard_API.DTO
{
    public class ToReturnToken
    {
     //عشان يرجع ك جيسون مش سترنغ   
        public string Token { get; set; }
        public DateTime Expired { get; set; }
    }
}

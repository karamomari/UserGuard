namespace UserGuard_API.Service
{
    public class ResendEmailService
    {
        private readonly HttpClient _httpClient;

        public ResendEmailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.resend.com");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "re_JfAzozLs_9GWHQnfMiTPsasjMfDxYKTbv");

        }

        public async Task<bool> SendEmailAsync(string toEmail,string subject,string Htmlcontent)
        {
            var payload = new
            {
                from = "Resend Demo <onboarding@resend.dev>", 
                to = new[] { toEmail },
                subject = subject,
                html = Htmlcontent
            };



            var response = await _httpClient.PostAsJsonAsync("/emails", payload);
            string content = await response.Content.ReadAsStringAsync();
   
            File.WriteAllText("C:\\Temp\\resend_log.txt", content);

            return response.IsSuccessStatusCode;
        }
    }
}

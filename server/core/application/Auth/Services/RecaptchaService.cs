using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace LocadoraDeAutomoveis.Application.Auth.Services;

public class RecaptchaService(
    HttpClient http,
    IConfiguration configuration
)
{
    public async Task<bool> VerifyRecaptchaToken(string token)
    {
        string url = "https://www.google.com/recaptcha/api/siteverify";

        string? key = configuration["CAPTCHA_KEY"];
        HttpResponseMessage response = await http.PostAsync(
            $"{url}?secret={key}&response={token}", null
        );

        response.EnsureSuccessStatusCode();

        RecaptchaResponse? jsonResponse = await response.Content.ReadFromJsonAsync<RecaptchaResponse>();

        return jsonResponse?.Success == true;
    }
}

public class RecaptchaResponse
{
    public bool Success { get; set; }
}

using System.Text;
using System.Xml.Linq;
using LoginForm.Models;

namespace LoginForm.Services;

public class RequestService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public RequestService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<(bool success, string message, EntityResponse? entity)> LoginAsync(string username, string password)
    {
        var soapUrl = _configuration["Mekashron:SoapUrl"];

        var soapEnvelope = $@"
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:ICUTech.Intf-IICUTech"">
                       <soapenv:Body>
                          <urn:Login>
                             <UserName>{username}</UserName>
                             <Password>{password}</Password>
                             <IPs></IPs>
                          </urn:Login>
                       </soapenv:Body>
                    </soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

        try
        {
            var response = await _httpClient.PostAsync(soapUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return (false, "HTTP Error: " + response.StatusCode, null);

            var doc = XDocument.Parse(responseString);
            var jsonText = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "return")?.Value;

            if (string.IsNullOrWhiteSpace(jsonText))
                return (false, "Invalid SOAP response", null);

            using var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonText);
            var root = jsonDoc.RootElement;
            
            if (root.TryGetProperty("ResultCode", out var codeProp))
            {
                var resultCode = codeProp.GetInt32();
                var resultMessage = root.GetProperty("ResultMessage").GetString();

                if (resultCode != 0)
                    return (false, resultMessage ?? "Login failed", null);
            }
            
            var entity = System.Text.Json.JsonSerializer.Deserialize<EntityResponse>(jsonText);
            return (true, "Login successful!", entity);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }
}
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace JiraIntegration;

[ApiController]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TestController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    [HttpGet("issues")]
    public async Task<IActionResult> GetIssues()
    {
        string jiraBaseUrl = "https://your-domain.atlassian.net"; // 替換為你的 Jira 網域
        string apiEndpoint = "/rest/api/3/search"; // Jira 搜尋端點
        string username = "your_email@example.com"; // Jira 帳號
        string apiToken = "your_api_token"; // Jira API Token

        // 創建 HttpClient
        var client = _httpClientFactory.CreateClient();

        // 設置基本的認證資訊
        var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{apiToken}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // 發送請求到 Jira API
        string url = $"{jiraBaseUrl}{apiEndpoint}";
        try
        {
            var response = await client.GetAsync(url);

            // 確認 API 是否成功
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                // 解析 JSON 回應
                var issues = JsonSerializer.Deserialize<JiraResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(issues); // 返回解析後的結果
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Jira API Error: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }

    }

    [HttpPost("issues")]
    public IActionResult CreateIssue([FromBody] object issue)
    {
        // 在這裡處理建立 Jira 任務的邏輯
        // 此處假設只是返回測試數據
        return Created("", new { Message = "Issue created successfully", Issue = issue });
    }
}

public class JiraResponse
{
    public Issue[] Issues { get; set; }
}

public class Issue
{
    public string Key { get; set; }
    public Fields Fields { get; set; }
}

public class Fields
{
    public string Summary { get; set; }
    public Status Status { get; set; }
}

public class Status
{
    public string Name { get; set; }
}
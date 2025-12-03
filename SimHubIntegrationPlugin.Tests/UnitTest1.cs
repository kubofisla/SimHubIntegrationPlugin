using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

using Loupedeck.SimHubIntegrationPlugin.Data;

namespace SimHubIntegrationPlugin.Tests;

public class DataLoaderTests
{
    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_handler(request));
    }

    [Fact]
    public void LoadDashboardData_PerformsGetAndReturnsJson()
    {
        // Arrange
        var expectedJson = JsonSerializer.Serialize(new { LastLapTime = "00:01:23.456" });
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler);
        var loader = new DataLoader("localhost", httpClient);

        // Act
        var json = loader.LoadDashboardData();

        // Assert
        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public void AdjustTargetBy_SendsPostWithCorrectDelta()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var httpClient = new HttpClient(handler);
        var loader = new DataLoader("localhost", httpClient);

        // Act
        var result = loader.AdjustTargetBy(0.5);

        // Assert
        Assert.True(result);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.EndsWith("/dashboarddata/adjust", capturedRequest!.RequestUri!.ToString());

        var body = capturedRequest.Content!.ReadAsStringAsync().Result;
        var doc = JsonDocument.Parse(body);
        Assert.Equal(0.5, doc.RootElement.GetProperty("delta").GetDouble());
    }

    [Fact]
    public void ResetTargetToFastestLap_SendsPostToCorrectEndpoint()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var httpClient = new HttpClient(handler);
        var loader = new DataLoader("localhost", httpClient);

        // Act
        var result = loader.ResetTargetToFastestLap();

        // Assert
        Assert.True(result);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.EndsWith("/dashboarddata/resettofast", capturedRequest!.RequestUri!.ToString());
    }

    [Fact]
    public void ResetTargetToLastLap_SendsPostToCorrectEndpoint()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var httpClient = new HttpClient(handler);
        var loader = new DataLoader("localhost", httpClient);

        // Act
        var result = loader.ResetTargetToLastLap();

        // Assert
        Assert.True(result);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.EndsWith("/dashboarddata/resettolast", capturedRequest!.RequestUri!.ToString());
    }
}

public class SimHubDataTests
{
    [Fact]
    public void ProcessNewData_UpdatesExistingBinding()
    {
        // Arrange
        var simHub = SimHubData.Instance;
        var key = EDataKey.TargetTime;
        var binding = new Binding<dynamic>(null);
        simHub.Data[key] = binding;

        var json = JsonSerializer.Serialize(new Dictionary<string, string>
        {
            { nameof(EDataKey.TargetTime), "120.5" }
        });

        // Act
        simHub.ProcessNewData(json);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(120.5), binding.Value);
    }
}

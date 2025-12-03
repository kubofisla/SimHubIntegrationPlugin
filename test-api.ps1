$baseUrl = "http://localhost:8080/dashboarddata"

function Test-Endpoint {
    param (
        [string]$Path,
        [string]$Method = "POST",
        [string]$Body = ""
    )

    $url = "$baseUrl/$Path"
    Write-Host "Testing $Method $url with body '$Body'..." -NoNewline

    try {
        $params = @{
            Uri        = $url
            Method     = $Method
            TimeoutSec = 2
        }

        if ($Method -eq "POST") {
            $params.ContentType = "application/json"
            $params.Body = $Body
        }

        $response = Invoke-RestMethod @params
        Write-Host " OK" -ForegroundColor Green
        if ($response) {
            Write-Host "Response: $($response | ConvertTo-Json -Depth 2)" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host " FAILED" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            Write-Host "Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        }
    }
}

Write-Host "=== SimHub API Diagnostic ==="
Test-Endpoint -Path "" -Method "GET"
Test-Endpoint -Path "resetto" -Body '{"target":"fast"}'
Test-Endpoint -Path "resetto" -Body '{"resetTo":"fast"}'
Test-Endpoint -Path "resettofast" -Method "POST" -Body "" # Will use default content type or none
Test-Endpoint -Path "resettolast" -Body ""

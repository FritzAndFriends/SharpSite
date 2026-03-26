$websiteUrl = "http://localhost:5020"  # Adjust the URL as needed

$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT="true"

# Delete the stop-aspire file if it exists
$stopAspireFilePath = Join-Path -Path "$PSScriptRoot/src/SharpSite.AppHost" -ChildPath "stop-aspire"
if (Test-Path -Path $stopAspireFilePath) {
	Remove-Item -Path $stopAspireFilePath -Force
}

# Run the .NET Aspire application in the background
$dotnetRunProcess = Start-Process -FilePath "dotnet" -ArgumentList "run -lp http --project src/SharpSite.AppHost/SharpSite.AppHost.csproj --testonly=true" -NoNewWindow -PassThru -RedirectStandardOutput "output.log" -RedirectStandardError "error.log"

# Function to check if the website is running
function Test-Website {
    param (
        [string]$url
    )
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 2
        return $true
    } catch {
        return $false
    }
}

# Wait for the website to be running
Write-Host "Waiting for the website to start at $websiteUrl ..." -ForegroundColor Yellow
$maxRetries = 90
$retryCount = 0
while (-not (Test-Website -url $websiteUrl) -and $retryCount -lt $maxRetries) {
    Start-Sleep -Seconds 2
    $retryCount++
    if ($retryCount % 15 -eq 0) {
        Write-Host "  Still waiting... ($retryCount/$maxRetries retries)" -ForegroundColor Yellow
    }
}

if ($retryCount -eq $maxRetries) {
    Write-Host "Website did not start within the expected time." -ForegroundColor Red
    Write-Host "--- AppHost stdout (last 50 lines) ---" -ForegroundColor Red
    if (Test-Path "output.log") { Get-Content "output.log" -Tail 50 }
    Write-Host "--- AppHost stderr (last 50 lines) ---" -ForegroundColor Red
    if (Test-Path "error.log") { Get-Content "error.log" -Tail 50 }

    # Stop the dotnet run process
    Stop-Process -Id $dotnetRunProcess.Id -Force
    exit 1
}

Write-Host "Website is running!" -ForegroundColor Green

# Change directory to the Playwright tests folder
# Set-Location -Path "$PSScriptRoot/e2e/SharpSite.E2E"

# Run Playwright tests using dotnet test
dotnet test ./e2e/SharpSite.E2E/SharpSite.E2E.csproj --logger trx --results-directory "playwright-test-results" -- xUnit.MaxParallelThreads=5

if ($LASTEXITCODE -ne 0) {
    Write-Host "Playwright tests failed!" -ForegroundColor Red


		# Create a file called stop-aspire
		$stopAspireFilePath = Join-Path -Path "$PSScriptRoot/src/SharpSite.AppHost" -ChildPath "stop-aspire"
		New-Item -Path $stopAspireFilePath -ItemType File -Force | Out-Null
		Set-Location -Path "$PSScriptRoot"
    exit $LASTEXITCODE
}

Write-Host "Build and tests completed successfully!" -ForegroundColor Green

# Stop the dotnet run process
	$stopAspireFilePath = Join-Path -Path "$PSScriptRoot/src/SharpSite.AppHost" -ChildPath "stop-aspire"
	New-Item -Path $stopAspireFilePath -ItemType File -Force | Out-Null

Set-Location -Path "$PSScriptRoot"

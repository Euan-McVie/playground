.\Login.ps1

Write-Host 'Deploying Load Test' -ForegroundColor Blue
New-AzResourceGroupDeploymentStack `
    -Name LoadTest `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\LoadTest\main.bicep `
    -Force

Write-Host 'Deployed Load Test' -ForegroundColor Blue

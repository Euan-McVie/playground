.\Login.ps1

Write-Host 'Deploying the EventHub' -ForegroundColor Blue
New-AzResourceGroupDeploymentStack `
    -Name EventHub `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\EventHub\main.bicep `
    -Force

Write-Host 'Deployed the EventHub' -ForegroundColor Blue

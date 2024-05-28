.\Login.ps1

Write-Host 'Destroying the EventHub' -ForegroundColor Magenta
Remove-AzResourceGroupDeploymentStack `
    -Name EventHub `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DeleteAll `
    -Force

Write-Host 'Destroyed the EventHub' -ForegroundColor Magenta

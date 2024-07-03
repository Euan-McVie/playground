.\Login.ps1

Write-Host 'Destroying Load Test' -ForegroundColor Blue
Remove-AzResourceGroupDeploymentStack `
    -Name LoadTest `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -ActionOnUnmanage DeleteAll `
    -Force

Write-Host 'Destroyed Load Test' -ForegroundColor Blue

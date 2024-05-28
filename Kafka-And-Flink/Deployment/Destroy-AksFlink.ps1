.\Login.ps1

Write-Host 'Destroying the Flink AKS Cluster' -ForegroundColor Magenta
Remove-AzResourceGroupDeploymentStack `
    -Name AksFlink `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DeleteAll `
    -Force

Write-Host 'Destroyed the Flink AKS Cluster' -ForegroundColor Magenta

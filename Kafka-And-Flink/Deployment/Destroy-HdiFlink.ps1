.\Login.ps1

Write-Host 'Destroying the HDInsights Flink Cluster' -ForegroundColor Magenta
Remove-AzResourceGroupDeploymentStack `
    -Name HdiFlink `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DeleteAll `
    -Force

Write-Host 'Destroyed the HDInsights Flink Cluster' -ForegroundColor Magenta

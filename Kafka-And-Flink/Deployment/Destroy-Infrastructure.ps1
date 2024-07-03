.\Login.ps1

Write-Host 'Destroying the Kafka and Flink infrastructure' -ForegroundColor Magenta
Remove-AzResourceGroupDeploymentStack `
    -Name Infrastructure `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -ActionOnUnmanage DeleteAll `
    -Force

Write-Host 'Destroying the Kafka and Flink resource group' -ForegroundColor Magenta
Remove-AzSubscriptionDeploymentStack `
    -Name KafkaAndFlink `
    -ActionOnUnmanage DeleteAll `
    -Force

Write-Host 'Destroyed the Kafka and Flink infrastructure' -ForegroundColor Magenta

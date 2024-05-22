#Login
.\Login.ps1

# Remove the shared infrastructure deployment stack
Remove-AzResourceGroupDeploymentStack `
    -Name Infrastructure `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DeleteAll `
    -Force

# Remove the resource group deployment stack
Remove-AzSubscriptionDeploymentStack `
    -Name KafkaAndFlink `
    -DeleteAll `
    -Force

#Login
.\Login.ps1

# Remove the Event Hub deployment stack
Remove-AzResourceGroupDeploymentStack `
    -Name EventHub `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DeleteAll `
    -Force

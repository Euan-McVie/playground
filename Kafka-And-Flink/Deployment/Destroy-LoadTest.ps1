#Login
.\Login.ps1

# Remove the Load Testing deployment stack
Remove-AzResourceGroupDeploymentStack `
    -Name LoadTest `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DeleteAll `
    -Force

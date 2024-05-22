#Login
.\Login.ps1

# Remove the Flink deployment stack
Remove-AzResourceGroupDeploymentStack `
    -Name HdiFlink `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DeleteAll `
    -Force

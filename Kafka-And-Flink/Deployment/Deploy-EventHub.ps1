#Login
.\Login.ps1

# Create the Event Hub deployment stack
New-AzResourceGroupDeploymentStack `
    -Name EventHub `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\EventHub\main.bicep `
    -Force

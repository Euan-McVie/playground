#Login
.\Login.ps1

# Create the Flink deployment stack
New-AzResourceGroupDeploymentStack `
    -Name HdiFlink `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\HdiFlink\main.bicep `
    -Force

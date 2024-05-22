#Login
.\Login.ps1

# Create the Load Testing deployment stack
New-AzResourceGroupDeploymentStack `
    -Name LoadTest `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\LoadTest\main.bicep `
    -Force

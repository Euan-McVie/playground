#Login
.\Login.ps1

# Create the Flink deployment stack
New-AzResourceGroupDeploymentStack `
    -Name AksFlink `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\AksFlink\main.bicep `
    -Force

Import-AzAksCredential -ResourceGroupName rg-euan-kafka-and-flink -Name flink-aks-euan-kafka-and-flink -Force


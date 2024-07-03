.\Login.ps1

Write-Host 'Deploying the Flink AKS Cluster' -ForegroundColor Blue
New-AzResourceGroupDeploymentStack `
    -Name AksFlink `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -ActionOnUnmanage DeleteAll `
    -TemplateFile .\AksFlink\main.bicep `
    -Force

Write-Host 'Importing the AKS Cluster Credentials to Kubectl' -ForegroundColor Blue
Import-AzAksCredential -ResourceGroupName rg-euan-kafka-and-flink -Name flink-aks-euan-kafka-and-flink -Force

Write-Host 'Deploying the Flink Operator' -ForegroundColor Blue
.\AksFlink\Deploy-Flink-Operator.ps1

Write-Host 'Deployed Flink' -ForegroundColor Blue

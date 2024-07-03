.\Login.ps1

Write-Host 'Deploying the HDInsights Flink Cluster' -ForegroundColor Blue
New-AzResourceGroupDeploymentStack `
    -Name HdiFlink `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -ActionOnUnmanage DeleteAll `
    -TemplateFile .\HdiFlink\main.bicep `
    -Force

Write-Host 'Deployed the HDInsights Flink Cluster' -ForegroundColor Blue

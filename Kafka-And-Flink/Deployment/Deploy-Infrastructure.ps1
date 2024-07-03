.\Login.ps1

$certs = .\Infrastructure\Generate-Certificates.ps1

Write-Host 'Deploying the Kafka and Flink resource group' -ForegroundColor Blue
New-AzSubscriptionDeploymentStack `
    -Name KafkaAndFlink `
    -Location uksouth `
    -DenySettingsMode DenyDelete `
    -ActionOnUnmanage DeleteAll `
    -TemplateFile .\Infrastructure\resourceGroup.bicep `
    -TemplateParameterFile .\Infrastructure\resourceGroup.bicepparam `
    -Force

$params = @{ vpnClientRootCert = [Convert]::ToBase64String($certs.RootCaCert.RawData) }
Write-Host 'Deploying the Kafka and Flink infrastructure' -ForegroundColor Blue
New-AzResourceGroupDeploymentStack `
    -Name Infrastructure `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -ActionOnUnmanage DeleteAll `
    -TemplateFile .\Infrastructure\main.bicep `
    -TemplateParameterObject $params `
    -Force

Write-Host 'Deployed the Kafka and Flink infrastructure' -ForegroundColor Blue

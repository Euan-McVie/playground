#Login
.\Login.ps1

# Generate the VPN Client Certificates
$certs = .\Infrastructure\Generate-Certificates.ps1

# Create the resource group deployment stack
New-AzSubscriptionDeploymentStack `
    -Name KafkaAndFlink `
    -Location uksouth `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\Infrastructure\resourceGroup.bicep `
    -TemplateParameterFile .\Infrastructure\resourceGroup.bicepparam `
    -Force

$params = @{ vpnClientRootCert = [Convert]::ToBase64String($certs.RootCaCert.RawData) }
# Create the shared infrastructure deployment stack
New-AzResourceGroupDeploymentStack `
    -Name Infrastructure `
    -ResourceGroupName rg-euan-kafka-and-flink `
    -DenySettingsMode DenyDelete `
    -DeleteAll `
    -TemplateFile .\Infrastructure\main.bicep `
    -TemplateParameterObject $params `
    -Force

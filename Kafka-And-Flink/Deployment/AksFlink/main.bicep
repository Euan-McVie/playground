param location string = resourceGroup().location

var common = loadJsonContent('../common.json')
var resourceSuffix = common.resourceSuffix
var pdDevSecurityGroupId = common.pdDevSecurityGroupId

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-09-01' existing = {
  name: 'vnet-${resourceSuffix}'
}

resource flinkSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' existing = {
  name: 'flink'
  parent: virtualNetwork
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = {
  name: 'log-${resourceSuffix}'
}

resource networkContributorRole 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  name: '4d97b98b-1d4f-4787-a291-c67834d212e7'
  scope: subscription()
}

resource flinkManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'id-flink-${resourceSuffix}'
  location: location
}

resource subnetRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(flinkManagedIdentity.id, flinkSubnet.id)
  scope: flinkSubnet
  properties: {
    roleDefinitionId: networkContributorRole.id
    principalId: flinkManagedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource aksCluster 'Microsoft.ContainerService/managedClusters@2024-01-01' = {
  name: 'flink-aks-${resourceSuffix}'
  location: location
  sku: {
    name: 'Base'
    tier: 'Free'
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${flinkManagedIdentity.id}': {}
    }
  }
  properties: {
    kubernetesVersion: '1.28.5'
    enableRBAC: true
    dnsPrefix: 'flink-aks-${resourceSuffix}'
    nodeResourceGroup: 'MC_${resourceGroup().name}_flink-aks-${resourceSuffix}'
    disableLocalAccounts: true
    aadProfile: {
      managed: true
      adminGroupObjectIDs: [pdDevSecurityGroupId]
      enableAzureRBAC: true
    }
    autoUpgradeProfile: {
      upgradeChannel: 'patch'
      nodeOSUpgradeChannel: 'NodeImage'
    }
    agentPoolProfiles: [
      {
        name: 'agentpool'
        osDiskSizeGB: 0
        count: 2
        enableAutoScaling: true
        minCount: 2
        maxCount: 3
        vmSize: 'Standard_D4ds_v5'
        osType: 'Linux'
        osSKU: 'Ubuntu'
        type: 'VirtualMachineScaleSets'
        mode: 'System'
        maxPods: 110
        nodeTaints: [
          'CriticalAddonsOnly=true:NoSchedule'
        ]
        enableNodePublicIP: false
        vnetSubnetID: flinkSubnet.id
      }
      {
        name: 'userpool'
        osDiskSizeGB: 0
        count: 2
        enableAutoScaling: true
        minCount: 2
        maxCount: 5
        vmSize: 'Standard_E4as_v5'
        osType: 'Linux'
        osSKU: 'Ubuntu'
        type: 'VirtualMachineScaleSets'
        mode: 'User'
        maxPods: 110
        enableNodePublicIP: false
        vnetSubnetID: flinkSubnet.id
      }
    ]
    apiServerAccessProfile: {
      enablePrivateCluster: false
    }
    addonProfiles: {
      azurePolicy: {
        enabled: true
      }
      azureKeyvaultSecretsProvider: {
        enabled: true
        config: {
          enableSecretRotation: 'false'
          rotationPollInterval: '2m'
        }
      }
    }
    supportPlan: 'KubernetesOfficial'
    networkProfile: {
      loadBalancerSku: 'standard'
      networkPlugin: 'azure'
      networkPolicy: 'azure'
      serviceCidr: '10.1.0.0/16'
      dnsServiceIP: '10.1.0.10'
    }
  }
}

resource logCollection 'Microsoft.Insights/dataCollectionRules@2022-06-01' = {
  name: 'ci-${aksCluster.name}'
  location: location
  kind: 'Linux'
  properties: {
    dataSources: {
      extensions: [
        {
          name: 'ContainerInsightsExtension'
          extensionName: 'ContainerInsights'
          streams: ['Microsoft-ContainerInsights-Group-Default']
          extensionSettings: {
            dataCollectionSettings: {
              interval: '5m'
              namespaceFilteringMode: 'exclude'
              enableContainerLogV2: true
              namespaces: [
                'kube-system'
                'gatekeeper-system'
                'azure-arc'
              ]
            }
          }
        }
      ]
    }
    destinations: {
      logAnalytics: [
        {
          name: 'ciworkspace'
          workspaceResourceId: logAnalyticsWorkspace.id
        }
      ]
    }
    dataFlows: [
      {
        streams: ['Microsoft-ContainerInsights-Group-Default']
        destinations: ['ciworkspace']
      }
    ]
  }
}

resource logCollectionAssociation 'Microsoft.Insights/dataCollectionRuleAssociations@2022-06-01' = {
  name: 'cia-${aksCluster.name}'
  scope: aksCluster
  properties: {
    dataCollectionRuleId: logCollection.id
  }
}

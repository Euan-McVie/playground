param location string = resourceGroup().location

var common = loadJsonContent('../common.json')
var resourceSuffix = common.resourceSuffix
var pdDevSecurityGroupId = common.pdDevSecurityGroupId

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-09-01' existing = {
  name: 'vnet-${resourceSuffix}'
}

resource defaultSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' existing = {
  name: 'default'
  parent: virtualNetwork
}

resource privateBlobDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: 'privatelink.blob.${environment().suffixes.storage}'
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = {
  name: 'log-${resourceSuffix}'
}

resource hdiClusterPool 'Microsoft.HDInsight/clusterpools@2023-11-01-preview' = {
  name: 'cp-${resourceSuffix}'
  location: location
  properties: {
    clusterPoolProfile: {
      clusterPoolVersion: '1.1'
    }
    computeProfile: {
      vmSize: 'Standard_F4s_v2'
    }
    networkProfile: {
      subnetId: defaultSubnet.id
      outboundType: 'loadBalancer'
      enablePrivateApiServer: true
    }
    logAnalyticsProfile: {
      enabled: true
      workspaceId: logAnalyticsWorkspace.id
    }
  }
}

resource flinkStorage 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'stflink${replace(common.resourceSuffix, '-', '')}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    isHnsEnabled: true
    supportsHttpsTrafficOnly: true
    allowSharedKeyAccess: false
    networkAcls: {
      defaultAction: 'Deny'
    }
  }
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2021-02-01' = {
  name: 'pep-${flinkStorage.name}'
  location: location
  properties: {
    subnet: {
      id: defaultSubnet.id
    }
    privateLinkServiceConnections: [
      {
        name: 'pl-${flinkStorage.name}'
        properties: {
          privateLinkServiceId: flinkStorage.id
          groupIds: [
            'blob'
          ]
        }
      }
    ]
  }
}

resource privateEndpointDnsGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2023-09-01' = {
  parent: privateEndpoint
  name: 'flinkStorageDnsGroup'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'dns-${flinkStorage.name}'
        properties: {
          privateDnsZoneId: privateBlobDnsZone.id
        }
      }
    ]
  }
}

resource flinkBlobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: flinkStorage
  name: 'default'
  properties: {}
}

resource flinkContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: flinkBlobService
  name: 'flink'
  properties: {
    publicAccess: 'None'
  }
}

resource flinkManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'id-flink-${resourceSuffix}'
  location: location
}

resource flinkStorageRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(flinkStorage.id, flinkManagedIdentity.id)
  scope: flinkStorage
  properties: {
    principalId: flinkManagedIdentity.properties.principalId
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'b7e6dc6d-f1e8-4753-8033-0f276bb0955b'
    )
  }
}

resource flinkCluster 'Microsoft.HDInsight/clusterpools/clusters@2023-11-01-preview' = {
  name: 'flink'
  parent: hdiClusterPool
  location: location
  properties: {
    clusterType: 'Flink'
    computeProfile: {
      nodes: [
        {
          type: 'head'
          vmSize: 'Standard_E8as_v5'
          count: 2
        }
        {
          type: 'worker'
          vmSize: 'Standard_E8as_v5'
          count: 3
        }
      ]
    }
    clusterProfile: {
      clusterVersion: '1.1.1'
      ossVersion: '1.17.0'
      identityProfile: {
        msiResourceId: flinkManagedIdentity.id
        msiClientId: flinkManagedIdentity.properties.clientId
        msiObjectId: flinkManagedIdentity.properties.principalId
      }
      authorizationProfile: {
        groupIds: [pdDevSecurityGroupId]
      }
      logAnalyticsProfile: {
        enabled: true
        applicationLogs: {
          stdErrorEnabled: true
          stdOutEnabled: true
        }
        metricsEnabled: true
      }
      flinkProfile: {
        deploymentMode: 'Session'
        jobManager: {
          cpu: 1
          memory: 2000
        }
        taskManager: {
          cpu: 2
          memory: 22348
        }
        historyServer: {
          cpu: 1
          memory: 2000
        }
        storage: {
          storageUri: 'abfs://${flinkContainer.name}@${flinkStorage.name}.blob.${environment().suffixes.storage}'
        }
      }
    }
  }
}

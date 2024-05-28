param location string = resourceGroup().location

var common = loadJsonContent('../common.json')
var resourceSuffix = common.resourceSuffix
var pdDevSecurityGroupId = common.pdDevSecurityGroupId

resource blobDataOwnerRole 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  name: 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b'
  scope: subscription()
}

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-09-01' existing = {
  name: 'vnet-${resourceSuffix}'
}

resource defaultSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' existing = {
  name: 'flink'
  parent: virtualNetwork
}

resource flinkSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' existing = {
  name: 'flink'
  parent: virtualNetwork
}

resource privateBlobDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: 'privatelink.blob.${environment().suffixes.storage}'
}

resource privateDfsDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: 'privatelink.dfs.${environment().suffixes.storage}'
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
      subnetId: flinkSubnet.id
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
    allowBlobPublicAccess: true
    isHnsEnabled: true
    supportsHttpsTrafficOnly: true
    allowSharedKeyAccess: false
    networkAcls: {
      defaultAction: 'Deny'
    }
  }
}

resource blobPrivateEndpoint 'Microsoft.Network/privateEndpoints@2021-02-01' = {
  name: 'pep-blob-${flinkStorage.name}'
  location: location
  properties: {
    subnet: {
      id: defaultSubnet.id
    }
    privateLinkServiceConnections: [
      {
        name: 'pl-blob-${flinkStorage.name}'
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

resource dfsPrivateEndpoint 'Microsoft.Network/privateEndpoints@2021-02-01' = {
  name: 'pep-dfs-${flinkStorage.name}'
  location: location
  properties: {
    subnet: {
      id: defaultSubnet.id
    }
    privateLinkServiceConnections: [
      {
        name: 'pl-dfs-${flinkStorage.name}'
        properties: {
          privateLinkServiceId: flinkStorage.id
          groupIds: [
            'dfs'
          ]
        }
      }
    ]
  }
}

resource blobPrivateEndpointDnsGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2023-09-01' = {
  parent: blobPrivateEndpoint
  name: 'privateEndpointDnsGroup'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'dns-blob-${flinkStorage.name}'
        properties: {
          privateDnsZoneId: privateBlobDnsZone.id
        }
      }
    ]
  }
}

resource dfsPrivateEndpointDnsGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2023-09-01' = {
  parent: dfsPrivateEndpoint
  name: 'privateEndpointDnsGroup'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'dns-dfs-${flinkStorage.name}'
        properties: {
          privateDnsZoneId: privateDfsDnsZone.id
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
    roleDefinitionId: blobDataOwnerRole.id
  }
}

resource devStorageRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(flinkStorage.id, pdDevSecurityGroupId)
  scope: flinkStorage
  properties: {
    principalId: pdDevSecurityGroupId
    roleDefinitionId: blobDataOwnerRole.id
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
          vmSize: 'Standard_D8d_v5'
          count: 2
        }
        {
          type: 'worker'
          vmSize: 'Standard_D8d_v5'
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
          storageUri: 'abfss://${flinkContainer.name}@${flinkStorage.name}.dfs.${environment().suffixes.storage}'
        }
      }
    }
  }
}

param location string = resourceGroup().location

var common = loadJsonContent('../common.json')
var resourceSuffix = common.resourceSuffix

resource loadTest 'Microsoft.LoadTestService/loadTests@2022-12-01' = {
  name: 'ldtest-${resourceSuffix}'
  location: location
}

resource loadTestKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: 'kv-ldtest-${resourceSuffix}'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        objectId: loadTest.identity.principalId
        tenantId: subscription().tenantId
        permissions: {
          keys: ['list', 'get']
          secrets: ['list', 'get']
          certificates: ['list', 'get']
        }
      }
    ]
  }
}

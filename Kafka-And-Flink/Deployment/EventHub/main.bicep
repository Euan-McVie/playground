param location string = resourceGroup().location

var common = loadJsonContent('../common.json')
var resourceSuffix = common.resourceSuffix

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-09-01' existing = {
  name: 'vnet-${resourceSuffix}'
}

resource defaultSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-09-01' existing = {
  name: 'default'
  parent: virtualNetwork
}

resource privateServiceBusDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: 'privatelink.servicebus.windows.net'
}

resource namespace 'Microsoft.EventHub/namespaces@2023-01-01-preview' = {
  name: 'evhns-${resourceSuffix}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
    capacity: 1
  }
  properties: {
    kafkaEnabled: true
    isAutoInflateEnabled: false
    maximumThroughputUnits: 0
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
    zoneRedundant: false
  }
}

resource namespaceNetworkRules 'Microsoft.EventHub/namespaces/networkrulesets@2024-01-01' = {
  parent: namespace
  name: 'default'
  properties: {
    publicNetworkAccess: 'Enabled'
    defaultAction: 'Allow'
    virtualNetworkRules: []
    ipRules: []
    trustedServiceAccessEnabled: false
  }
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2021-02-01' = {
  name: 'pep-${namespace.name}'
  location: location
  properties: {
    subnet: {
      id: defaultSubnet.id
    }
    privateLinkServiceConnections: [
      {
        name: 'pl-${namespace.name}'
        properties: {
          privateLinkServiceId: namespace.id
          groupIds: [
            'namespace'
          ]
        }
      }
    ]
  }
}

resource privateEndpointDnsGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2023-09-01' = {
  parent: privateEndpoint
  name: 'serviceBusDnsGroup'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'dns-${namespace.name}'
        properties: {
          privateDnsZoneId: privateServiceBusDnsZone.id
        }
      }
    ]
  }
}

resource eventHubInput 'Microsoft.EventHub/namespaces/eventhubs@2024-01-01' = {
  parent: namespace
  name: 'public-orders'
  properties: {
    messageRetentionInDays: 7
    partitionCount: 2
    status: 'Active'
  }
}

resource eventHubInputMarketDepthCalculatorConsumerGroup 'Microsoft.EventHub/namespaces/eventhubs/consumergroups@2024-01-01' = {
  parent: eventHubInput
  name: 'market_depth_calculator'
}

resource eventHubInputMarketDepthCalculatorSasKey 'Microsoft.EventHub/namespaces/eventhubs/authorizationrules@2024-01-01' = {
  parent: eventHubInput
  name: 'market_depth_calculator'
  properties: {
    rights: [
      'Listen'
    ]
  }
}

resource eventHubOutput 'Microsoft.EventHub/namespaces/eventhubs@2024-01-01' = {
  parent: namespace
  name: 'market-depth'
  properties: {
    messageRetentionInDays: 7
    partitionCount: 2
    status: 'Active'
  }
}

resource eventHubOutputMarketDepthCalculatorSasKey 'Microsoft.EventHub/namespaces/eventhubs/authorizationrules@2024-01-01' = {
  parent: eventHubOutput
  name: 'market_depth_calculator'
  properties: {
    rights: [
      'Send'
    ]
  }
}

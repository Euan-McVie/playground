targetScope = 'subscription'

param location string = deployment().location

resource rgKafkaAndFlink 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: 'rg-euan-kafka-and-flink'
  location: location
}

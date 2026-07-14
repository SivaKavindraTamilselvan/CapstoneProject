param location string = resourceGroup().location
param uniqueSuffix string = uniqueString(resourceGroup().id)

@description('Key Vault name you already created')
param existingKeyVaultName string

var functionStorageName = 'ecommfn${uniqueSuffix}'
var functionPlanName = 'ecommerce-func-plan'
var functionAppName = 'ecommerce-func-${uniqueSuffix}'

resource functionStorage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: functionStorageName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
}

resource functionPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: functionPlanName
  location: location
  sku: { name: 'B1', tier: 'Basic' }   // changed from Y1/Dynamic
  properties: { reserved: true }
}

resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: functionPlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      appSettings: [
        { name: 'AzureWebJobsStorage', value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};AccountKey=${functionStorage.listKeys().keys[0].value};EndpointSuffix=core.windows.net' }
        { name: 'FUNCTIONS_WORKER_RUNTIME', value: 'dotnet-isolated' }
        { name: 'FUNCTIONS_EXTENSION_VERSION', value: '~4' }
        { name: 'KeyVaultName', value: existingKeyVaultName }
      ]
    }
  }
}

output functionAppName string = functionApp.name
output functionPrincipalId string = functionApp.identity.principalId
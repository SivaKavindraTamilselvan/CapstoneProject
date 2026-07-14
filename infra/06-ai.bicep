param location string = resourceGroup().location
param uniqueSuffix string = uniqueString(resourceGroup().id)

@description('Key Vault name you already created')
param existingKeyVaultName string

var aiPlanName = 'ecommerce-ai-plan'
var aiAppName = 'ecommerce-ai-${uniqueSuffix}'

resource aiServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: aiPlanName
  location: location
  sku: { name: 'B1', tier: 'Basic' }
  properties: { reserved: true }
}

resource aiApp 'Microsoft.Web/sites@2023-12-01' = {
  name: aiAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: aiServicePlan.id
    siteConfig: {
      linuxFxVersion: 'PYTHON|3.12'
      appSettings: [
        { name: 'KeyVaultName', value: existingKeyVaultName }
        { name: 'SCM_DO_BUILD_DURING_DEPLOYMENT', value: 'true' }
      ]
    }
  }
}

output aiAppName string = aiApp.name
output aiAppUrl string = 'https://${aiApp.properties.defaultHostName}'
output aiPrincipalId string = aiApp.identity.principalId
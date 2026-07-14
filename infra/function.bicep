// ===================================================================
// CapStoneProject_Ecommerce — Full Cloud Infrastructure
// Resource Group target: rg-sivakavindra
// ===================================================================

param location string = resourceGroup().location
param uniqueSuffix string = uniqueString(resourceGroup().id)

@description('Postgres admin login')
param pgAdminLogin string = 'sivakavindra'

@secure()
param pgAdminPassword string

@description('Base name used to derive all resource names')
param baseName string = 'ecommerce'

var pgServerName = '${baseName}-pg-${uniqueSuffix}'
var pgDbName = 'Ecommerce'
var kvName = '${baseName}-kv-${uniqueSuffix}'
var storageName = '${baseName}sa${uniqueSuffix}'
var productImagesContainer = 'product-images'
var appServicePlanName = '${baseName}-plan'
var apiAppName = '${baseName}-api-${uniqueSuffix}'
var functionAppName = '${baseName}-func-${uniqueSuffix}'
var functionStorageName = '${baseName}fnsa${uniqueSuffix}'
var auditQueueName = 'audit-events'
var refundQueueName = 'refund-queue'

// -------------------------------------------------------------
// 1. Postgres Flexible Server + Database
// -------------------------------------------------------------
resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-06-01-preview' = {
  name: pgServerName
  location: location
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    version: '16'
    administratorLogin: pgAdminLogin
    administratorLoginPassword: pgAdminPassword
    storage: { storageSizeGB: 32 }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
    highAvailability: { mode: 'Disabled' }
  }
}

resource postgresDb 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-06-01-preview' = {
  parent: postgresServer
  name: pgDbName
  properties: {
    charset: 'UTF8'
    collation: 'en_US.utf8'
  }
}

resource pgAllowAzureServices 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-06-01-preview' = {
  parent: postgresServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// -------------------------------------------------------------
// 2. Key Vault (RBAC-enabled — no access policies, pure RBAC)
// -------------------------------------------------------------
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: kvName
  location: location
  properties: {
    sku: { family: 'A', name: 'standard' }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
  }
}

// Postgres connection string stored as a secret (SSL required for Flexible Server)
resource pgConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'PostgresConnectionString'
  properties: {
    value: 'Host=${postgresServer.properties.fullyQualifiedDomainName};Port=5432;Database=${pgDbName};Username=${pgAdminLogin};Password=${pgAdminPassword};SSL Mode=Require;Trust Server Certificate=true'
  }
}

// -------------------------------------------------------------
// 3. Storage Account — Blob (product images) + Queues (audit, refund)
// -------------------------------------------------------------
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: { accessTier: 'Hot' }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

resource productImagesBlobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: productImagesContainer
  properties: { publicAccess: 'Blob' }
}

resource queueService 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

resource auditQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2023-01-01' = {
  parent: queueService
  name: auditQueueName
}

resource refundQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2023-01-01' = {
  parent: queueService
  name: refundQueueName
}

// Blob connection string stored in Key Vault (never in appsettings.json)
resource blobConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'BlobStorageConnectionString'
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
  }
}

// JWT signing key + Razorpay + Shiprocket secrets — also centralised in Key Vault
@secure()
param jwtKey string
@secure()
param razorpayKeySecret string
param razorpayKeyId string

resource jwtSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'JwtKey'
  properties: { value: jwtKey }
}

resource razorpaySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'RazorpayKeySecret'
  properties: { value: razorpayKeySecret }
}

// -------------------------------------------------------------
// 4. App Service Plan + Web App (API) — Linux, .NET 8/10
// -------------------------------------------------------------
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: { name: 'B1', tier: 'Basic' }
  properties: { reserved: true } // Linux
}

resource apiApp 'Microsoft.Web/sites@2023-12-01' = {
  name: apiAppName
  location: location
  identity: {
    type: 'SystemAssigned' // Managed Identity — no passwords
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      appSettings: [
        { name: 'KeyVaultName', value: kvName }
        { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
        { name: 'RazorpayKeyId', value: razorpayKeyId }
      ]
    }
  }
}

// Grant API's Managed Identity read access to Key Vault secrets
resource apiKvRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, apiApp.id, 'KeyVaultSecretsUser')
  scope: keyVault
  properties: {
    principalId: apiApp.identity.principalId
    principalType: 'ServicePrincipal'
    // Key Vault Secrets User role
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
  }
}

// Grant API's Managed Identity Storage Blob Data Contributor (for direct SDK blob access without connection string, optional hardening)
resource apiStorageRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, apiApp.id, 'StorageBlobDataContributor')
  scope: storageAccount
  properties: {
    principalId: apiApp.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  }
}

// -------------------------------------------------------------
// 5. Function App — audit archive, abandoned cart, vendor purge,
//    payouts (timer), refund processor (queue trigger)
// -------------------------------------------------------------
resource functionStorage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: functionStorageName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
}

resource functionPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${baseName}-func-plan'
  location: location
  sku: { name: 'Y1', tier: 'Dynamic' } // Consumption plan
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
        { name: 'KeyVaultName', value: kvName }
        { name: 'RefundQueueStorage', value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net' }
      ]
    }
  }
}

resource funcKvRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, functionApp.id, 'KeyVaultSecretsUser')
  scope: keyVault
  properties: {
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
  }
}

resource funcQueueRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, functionApp.id, 'StorageQueueDataContributor')
  scope: storageAccount
  properties: {
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
    // Storage Queue Data Contributor
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '974c5e8b-45b9-4653-ba55-5f855dd0fb88')
  }
}

// -------------------------------------------------------------
// Outputs
// -------------------------------------------------------------
output postgresHost string = postgresServer.properties.fullyQualifiedDomainName
output keyVaultName string = kvName
output storageAccountName string = storageAccount.name
output apiAppName string = apiApp.name
output apiAppUrl string = 'https://${apiApp.properties.defaultHostName}'
output functionAppName string = functionApp.name

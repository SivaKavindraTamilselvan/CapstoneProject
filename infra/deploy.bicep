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

@description('Your local public IPv4 address, needed to run EF Core migrations from your machine. Get it via: curl -4 ifconfig.me')
param myLocalIp string

resource pgAllowMyIp 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-06-01-preview' = {
  parent: postgresServer
  name: 'AllowMyLocalIp'
  properties: {
    startIpAddress: myLocalIp
    endIpAddress: myLocalIp
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
param shiprocketEmail string
@secure()
param shiprocketPassword string

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

resource shiprocketEmailSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'ShiprocketEmail'
  properties: { value: shiprocketEmail }
}

resource shiprocketPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'ShiprocketPassword'
  properties: { value: shiprocketPassword }
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
        { name: 'RazorpayKeySecret', value: razorpayKeySecret }
        { name: 'ShiprocketEmail', value: shiprocketEmail }
        { name: 'ShiprocketPassword', value: shiprocketPassword }
        { name: 'JwtKey', value: jwtKey }
        { name: 'ConnectionStrings__Default', value: 'Host=${postgresServer.properties.fullyQualifiedDomainName};Port=5432;Database=${pgDbName};Username=${pgAdminLogin};Password=${pgAdminPassword};SSL Mode=Require;Trust Server Certificate=true' }
        { name: 'BlobStorageConnectionString', value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net' }
      ]
    }
  }
}

// NOTE: Role assignments removed — this subscription account lacks
// Microsoft.Authorization/roleAssignments/write permission (common in
// restricted training subscriptions). Secrets are passed directly as
// App Settings below instead of via Managed Identity + Key Vault reference.
// If you later get "User Access Administrator" or "Owner" on this resource
// group, these can be re-added for a fully passwordless setup.

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

// NOTE: Role assignments removed for the same permissions reason as above.
// The Function App reads secrets via app settings instead of Key Vault + Managed Identity.

// -------------------------------------------------------------
// Outputs
// -------------------------------------------------------------
output postgresHost string = postgresServer.properties.fullyQualifiedDomainName
output keyVaultName string = kvName
output storageAccountName string = storageAccount.name
output apiAppName string = apiApp.name
output apiAppUrl string = 'https://${apiApp.properties.defaultHostName}'
output functionAppName string = functionApp.name

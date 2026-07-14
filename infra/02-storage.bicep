// =========================================================
// STEP 2 — Storage Account (Blob + Queues)
// Assumes you already created your Key Vault manually via the
// CLI lab. Pass its name in as a parameter so this script can
// write the connection string secret into it.
// =========================================================

param location string = resourceGroup().location
param uniqueSuffix string = uniqueString(resourceGroup().id)

@description('Name of the Key Vault you already created (e.g. sivakavindra14426)')
param existingKeyVaultName string

// Storage account names: 3-24 chars, lowercase letters + numbers only.
// 'ecommsa' (7) + uniqueSuffix (13) = 20 chars — safely under the limit.
var storageName = 'ecommsa${uniqueSuffix}'
var productImagesContainer = 'product-images'
var auditQueueName = 'audit-events'
var refundQueueName = 'refund-queue'

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
  properties: { publicAccess: 'None' } // subscription policy blocks public blob access — serve via API or SAS tokens instead
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

// Reference your existing vault (not creating a new one — you already made it)
resource existingKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: existingKeyVaultName
}

resource blobConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'BlobStorageConnectionString'
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
  }
}

output storageAccountName string = storageAccount.name

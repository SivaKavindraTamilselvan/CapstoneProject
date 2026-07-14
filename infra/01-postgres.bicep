param location string = resourceGroup().location
param uniqueSuffix string = uniqueString(resourceGroup().id)

param pgAdminLogin string = 'sivakavindra'

@secure()
param pgAdminPassword string

@description('Your public IPv4 address — get it via: curl -4 ifconfig.me')
param myLocalIp string

@description('Name of the Key Vault you already created')
param existingKeyVaultName string

var pgServerName = 'ecommerce-pg-${uniqueSuffix}'
var pgDbName = 'Ecommerce'

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

resource pgAllowMyIp 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-06-01-preview' = {
  parent: postgresServer
  name: 'AllowMyLocalIp'
  properties: {
    startIpAddress: myLocalIp
    endIpAddress: myLocalIp
  }
}

output pgServerName string = pgServerName
output pgHost string = postgresServer.properties.fullyQualifiedDomainName

resource existingKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: existingKeyVaultName
}

resource pgConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'PostgresConnectionString'
  properties: {
    value: 'Host=${postgresServer.properties.fullyQualifiedDomainName};Port=5432;Database=${pgDbName};Username=${pgAdminLogin};Password=${pgAdminPassword};SSL Mode=Require;Trust Server Certificate=true'
  }
}
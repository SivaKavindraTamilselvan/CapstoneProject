// =========================================================
// STEP 2c — Email + AI microservice secrets into Key Vault
// =========================================================
param existingKeyVaultName string

param emailSmtpHost string
param emailSmtpPort string
param emailSenderName string
param emailSenderEmail string
param emailUsername string
@secure()
param emailPassword string
param emailUseSsl string = 'true'

@secure()
param groqApiKey string

resource existingKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: existingKeyVaultName
}

resource emailSmtpHostSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'EmailSmtpHost'
  properties: { value: emailSmtpHost }
}

resource emailSmtpPortSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'EmailSmtpPort'
  properties: { value: emailSmtpPort }
}

resource emailSenderNameSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'EmailSenderName'
  properties: { value: emailSenderName }
}

resource emailSenderEmailSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'EmailSenderEmail'
  properties: { value: emailSenderEmail }
}

resource emailUsernameSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'EmailUsername'
  properties: { value: emailUsername }
}

resource emailPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'EmailPassword'
  properties: { value: emailPassword }
}

resource emailUseSslSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'EmailUseSsl'
  properties: { value: emailUseSsl }
}

resource groqApiKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: existingKeyVault
  name: 'GroqApiKey'
  properties: { value: groqApiKey }
}
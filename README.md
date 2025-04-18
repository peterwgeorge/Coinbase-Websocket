# Coinbase-Websocket

A C# implementation for connecting to the Coinbase Advanced Trading API WebSocket feed and visualizing the data with React. This project was created as a learning exercise to understand how to properly establish and maintain WebSocket connections with the Coinbase API using ECDSA or Ed25519 signature authentication.

## Overview

This project demonstrates how to:

- Connect to Coinbase's Advanced Trading WebSocket API
- Authenticate using ECDSA and Ed25519 with JWT tokens
- Manage WebSocket connection lifecycle
- Securely store API credentials using AWS Secrets Manager
- Process real-time market data from Coinbase
- Use Factory pattern for signature algorithm selection
- Use Dependency Injection in .NET

## Prerequisites

- .NET SDK (compatible with the project version)
- AWS account with Secrets Manager configured
- Coinbase Advanced Trading API credentials (saved in AWS Secrets Manager)

## Setup

1. Clone the repository:
   ```
   git clone https://github.com/peterwgeorge/Coinbase-Websocket.git
   cd Coinbase-Websocket
   ```

2. Configure AWS credentials:
- Make sure you have the AWS CLI installed and configured with credentials that have access to Secrets Manager
- Alternatively, you can configure the AWS SDK through environment variables:
  ```
  export AWS_ACCESS_KEY_ID="your-access-key"
  export AWS_SECRET_ACCESS_KEY="your-secret-key"
  export AWS_REGION="your-region"
  ```
- For development environments, you can also use the credentials file in `~/.aws/credentials`

3. Configure AWS Secrets Manager:
- Store your Coinbase API credentials in a JSON with this structure:
  ```json
  {
    "algorithm": "ed25519 or ecdsa",
    "keyId": "your-key-id", 
    "secret": "your-secret"
  }
  ```
- Note the name of your secret in AWS as you'll need it in the next step

4. Modify `appsettings.json`
   - Locate the `appsettings.json` file
   - Modify it to match the name of your secret in AWS Secrets Manager, AWS region, and desired URLs for WebSocket server and React App.

5. Restore NuGet packages:
   ```
   dotnet restore
   ```
6. Build the project:
   ```
   dotnet build
   ```

7. Run the WebsocketServer application:
   ```
   cd WebsocketServer
   dotnet run
   ```
8. Run the React app
   ```
   cd coinbase-visualizer
   pnpm dev
   ```
9. Visit the URL provided by Vite after starting the development server..

## Coinbase API Credentials

This project assumes you have created API credentials in Coinbase Advanced Trading and and have selected either ECDSA or Ed25519 as the signing algorithm when creating your API key. Refer to the Setup section for the expected JSON format for storage in AWS Secrets Manager.

## Implementation Details

### Security Improvements

The original JWT generation code from Coinbase's examples has been modified to use `RandomNumberGenerator` instead of `Random` for improved cryptographic security. We use Jose-JWT with System.Security.Cryptography for EcDSA and manually create a JWT in conjunction with NSec.Cryptography for Ed25519

### AWS Integration

The project leverages AWS Secrets Manager to securely store your Coinbase API credentials. This approach avoids hardcoding sensitive information in your source code.

### WebSocket Connection

The application handles WebSocket connection establishment, authentication, subscription to channels, and processing of incoming messages.

### Future Improvements

- Improved UI, with ability to select different ticker values, multiple exchanges, and color coding

## Dependencies

The project uses the following NuGet packages:
- AWSSDK.SecretsManager
- Newtonsoft.Json
- Microsoft.IdentityModel.Tokens
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Binder
- Microsoft.Extensions.Configuration.Json
- System.IdentityModel.Tokens.Jwt
- Jose-JWT
- NSec.Cryptography

## Troubleshooting

- If you encounter connection issues, verify that your API credentials are correctly stored in AWS Secrets Manager
- Ensure the secret name in `SecretsProvider.cs` matches your AWS secret name
- Check that your Coinbase API key has the appropriate permissions
- Ensure your Coinbase API key uses the ECDSA algorithm 

## Disclaimer

This project is for educational purposes. It is not officially affiliated with Coinbase. Always follow Coinbase's API usage guidelines and terms of service.


*Note: This project was created as a learning exercise to understand the Coinbase Advanced Trading API WebSocket implementation.*

# Coinbase-Websocket

A C# implementation for connecting to the Coinbase Advanced Trading API WebSocket feed. This project was created as a learning exercise to understand how to properly establish and maintain WebSocket connections with the Coinbase API using ECDSA signature authentication.

## Overview

This project demonstrates how to:

- Connect to Coinbase's Advanced Trading WebSocket API
- Authenticate using ECDSA signatures and JWT tokens
- Manage WebSocket connection lifecycle
- Securely store API credentials using AWS Secrets Manager
- Process real-time market data from Coinbase

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

2. Configure AWS Secrets Manager:
   - Store your Coinbase API credentials JSON in AWS Secrets Manager
   - Note the name of your secret as you'll need it in the next step

3. Update the secret name in `SecretsProvider.cs`:
   - Locate the `secretName` variable in the file
   - Change it to match the name of your secret in AWS Secrets Manager

4. Restore NuGet packages:
   ```
   dotnet restore
   ```

5. Build the project:
   ```
   dotnet build
   ```

6. Run the application:
   ```
   dotnet run
   ```

## Coinbase API Credentials

This project assumes you have created API credentials in Coinbase Advanced Trading and have selected the ECDSA signature algorithm. The expected format of your AWS secret is the JSON provided by Coinbase when you created your API key.

## Implementation Details

### Security Improvements

The original JWT generation code from Coinbase's examples has been modified to use `RandomNumberGenerator` instead of `Random` for improved cryptographic security.

### AWS Integration

The project leverages AWS Secrets Manager to securely store your Coinbase API credentials. This approach avoids hardcoding sensitive information in your source code.

### WebSocket Connection

The application handles WebSocket connection establishment, authentication, subscription to channels, and processing of incoming messages.

## Dependencies

The project uses the following NuGet packages:
- AWSSDK.SecretsManager
- Newtonsoft.Json
- Microsoft.IdentityModel.Tokens
- System.IdentityModel.Tokens.Jwt
- Jose-JWT

## Troubleshooting

- If you encounter connection issues, verify that your API credentials are correctly stored in AWS Secrets Manager
- Ensure the secret name in `SecretsProvider.cs` matches your AWS secret name
- Check that your Coinbase API key has the appropriate permissions
- Ensure your Coinbase API key uses the ECDSA algorithm 

## Disclaimer

This project is for educational purposes. It is not officially affiliated with Coinbase. Always follow Coinbase's API usage guidelines and terms of service.


*Note: This project was created as a learning exercise to understand the Coinbase Advanced Trading API WebSocket implementation.*

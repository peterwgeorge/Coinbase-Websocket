# CryptoExchange-Websocket

A C# implementation for connecting to cryptocurrency exchange WebSocket APIs—starting with Coinbase and Binance—and visualizing real-time market data in a React frontend. This project was built as a learning tool to explore WebSocket communication, secure API integration, and data visualization across multiple crypto exchanges.

## Overview

This project demonstrates how to:

- Connect to WebSocket APIs from exchanges like Coinbase and Binance
- Authenticate using ECDSA and Ed25519 signatures (Coinbase only)
- Manage WebSocket connection lifecycles for multiple exchanges
- Securely store API credentials using AWS Secrets Manager
- Normalize and display live price data across exchanges in a shared UI
- Use the Factory Pattern to support multiple authentication mechanisms
- Apply Dependency Injection principles in a .NET environment

## Prerequisites

- .NET SDK (compatible with the project version)
- AWS account with Secrets Manager configured
- Exchange API credentials (e.g., Coinbase, Binance) stored in AWS Secrets Manager

## Setup

1. Clone the repository:
   ```
   git clone https://github.com/peterwgeorge/CryptoExchange-Websocket.git
   cd CryptoExchange-Websocket
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
8. Create/set .env file with VITE_RELAY_SERVER_URL=ws://url-you-set-for-WebsocketServer/ws

9. Run the React app
   ```
   cd frontend
   pnpm dev
   ```

10. Visit the URL provided by Vite after starting the development server. 

## Exchange Credentials

Currently, authentication and signed requests are only required for Coinbase. Binance connections use unauthenticated public feeds. More exchanges will be added with support for their respective auth mechanisms.

## Implementation Details

### Security Improvements

The original JWT generation code from Coinbase's examples has been modified to use `RandomNumberGenerator` instead of `Random` for improved cryptographic security. We use Jose-JWT with System.Security.Cryptography for EcDSA and manually create a JWT in conjunction with NSec.Cryptography for Ed25519

### AWS Integration

The project leverages AWS Secrets Manager to securely store your Coinbase API credentials. This approach avoids hardcoding sensitive information in your source code.

### WebSocket Connection

The application handles WebSocket connection establishment, authentication, subscription to channels, and processing of incoming messages.

## Future Improvements

- Add support for Kraken, Gemini, and other exchanges
- Extend WebSocket subscription options (order books, trades, etc.)
- Improve frontend UI/UX
- Add price smoothing and anomaly detection
- Evaluate migrating backend from C# to Python or Rust

## Dependencies

.NET
 - AWSSDK.SecretsManager
 - Newtonsoft.Json
 - Microsoft.IdentityModel.Tokens
 - System.IdentityModel.Tokens.Jwt
 - Jose-JWT
 - NSec.Cryptography

Frontend
 - React
 - Vite
 - Recharts

## Troubleshooting

- Ensure your AWS secret names and credentials are correct.
- Confirm that your API key matches the expected algorithm and has necessary permissions.
- Make sure the WebSocket server is reachable from the frontend.
- Check CORS and proxy settings if using a non-local environment.

## Disclaimer

This project is for educational purposes only and is not affiliated with any of the supported exchanges. Please review and comply with each exchange’s API usage policies.

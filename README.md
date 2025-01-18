Beginner Level: JWT Integration

This project demonstrates the Beginner level implementation of JWT (JSON Web Token) authentication in a .NET 8 Web API. It covers basic token generation, token validation, secure and public endpoints, and unit testing.

Features

JWT Token Generation: Secure token creation with configurable SecretKey, Issuer, and Audience.

Token Validation: Middleware for validating JWTs in requests.

Secure and Public Endpoints:

GET /api/auth/secure-endpoint: Requires a valid JWT.

GET /api/auth/public-endpoint: Accessible without authentication.

Unit Tests: Comprehensive test coverage for token generation and validation.

Swagger Integration: Supports Bearer Token authorization in Swagger UI.

Prerequisites

Ensure you have the following installed on your system:

.NET SDK 8.0 or later

A code editor like Visual Studio, Rider, or VS Code

Getting Started

Clone the Repository

git clone <repository-url>
cd JWTIntegration

Build the Solution

dotnet build

Run the Project

To start the BeginnerAPI project:

dotnet run --project src/BeginnerAPI

The API will be available at http://localhost:5000.

Endpoints

Token Generation

POST /api/auth/generate-token

Description: Generates a JWT token.

Request:
No payload required.

Response:

{
  "token": "<JWT_TOKEN>"
}

Secure Endpoint

GET /api/auth/secure-endpoint

Description: A secure endpoint that requires a valid JWT.

Authorization:
Add Bearer <JWT_TOKEN> in the Authorization header.

Response:

{
  "message": "This is a secure endpoint. You have a valid token!",
  "timestamp": "<UTC_TIMESTAMP>"
}

Public Endpoint

GET /api/auth/public-endpoint

Description: A public endpoint accessible without authentication.

Response:

{
  "message": "This is a public endpoint. No token required!",
  "timestamp": "<UTC_TIMESTAMP>"
}

Testing

Unit tests are implemented using xUnit to validate the functionality of token generation and validation.

Run Tests

dotnet test

Test Coverage

TokenServiceTests:

GenerateToken_ShouldReturnValidJwtToken

ValidateToken_ShouldPassForValidToken

ValidateToken_ShouldFailForInvalidSignature

Technologies Used

.NET 8 Web API

JWT Authentication

Swagger (Swashbuckle)

xUnit for Unit Testing

FluentAssertions for Test Assertions

Next Steps

The Beginner level is complete! Future levels (Intermediate, Advanced, and Professional) will expand on this foundation by introducing more advanced JWT features and patterns.

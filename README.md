![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Docker](https://img.shields.io/badge/Docker-Multi--Service-blue)
![API Documentation](https://img.shields.io/badge/API-Swagger%20%2B%20OpenAPI-orange)
![Microservices](https://img.shields.io/badge/Architecture-Microservices-green)
![Communication](https://img.shields.io/badge/Protocols-HTTP%20%2B%20gRPC%20%2B%20Bridge-blue)
![AI Integration](https://img.shields.io/badge/AI-HuggingFace%20%2B%20Llama-orange)
![License](https://img.shields.io/badge/License-MIT-blue)

<!-- Logo -->
<p align="center">
  <img src="logo.png" alt="SportMatrix Logo" width="200"/>
</p>

<h1 align="center">🏋️‍♀️ SportMatrix</h1>
<p align="center">
  A comprehensive fitness analytics platform with AI-powered insights, multi-protocol communication, and clean architecture.
</p>

---

## 🎯 Project Goals & Roadmap

This project is a modern software development platform focusing on:

### 🏗️ Architecture & Design
- **Clean Architecture** with Domain, Application, Infrastructure Layering
- **Domain-Driven Design** principles for domain modeling
- **Microservices Architecture** with Multi-Protocol Communication
- **Event-Driven Architecture** with RabbitMQ for loosely coupled services

### 🔄 Multi-Protocol Communication
- **HTTP/REST** for standard API integration
- **Native gRPC** for high-performance service-to-service communication  
- **gRPC-JSON Bridge** as a best-of-both-worlds solution
- **Message Queuing** with RabbitMQ for asynchronous processing

### 🤖 AI & Modern Technologies
- **AI Integration** with HuggingFace and Google Gemini for intelligent training analysis
- **ASP.NET Core Razor Pages Frontend** with modern UI patterns
- **Real-time Health Monitoring** with comprehensive dashboard

### 🧪 Code Quality & Testing
- **Architecture Tests** with NetArchTest for Clean Architecture compliance
- **Modularity Maturity Index** calculation for sustainable code quality
- **Continuous Code Quality** with SonarCloud integration
- **Comprehensive Testing** with unit, integration, and architecture tests

### 📋 Roadmap

**✅ Currently Implemented:**
- Clean Architecture with strict dependency inversion
- Multi-Protocol Communication (HTTP, gRPC, gRPC-JSON)
- AI Integration with HuggingFace + Google Gemini
- Health Monitoring Dashboard with auto-refresh
- Comprehensive Error Handling with custom exception hierarchy
- Docker Multi-Service Setup

**📋 Planned:**
- Event-Driven Architecture with RabbitMQ
- CQRS Pattern for better read/write separation
- Modularity Maturity Index Integration
- Extended microservices with domain events

---

## 🎯 Dashboard Overview

![Sport Analytics Dashboard](./docs/images/Dashboard.png)
![Activity Distribution](./docs/images/ActivityDistribution.png)

---

## 🔬 Code Quality & Security

This project uses **SonarCloud** for continuous code quality monitoring:
- 🛡️ **Security Vulnerabilities** - Automated security scanning
- 🐛 **Bug Detection** - Potential bugs are identified
- 📊 **Code Coverage** - Test coverage is measured
- 🧹 **Code Smells** - Maintainability is evaluated
- 📈 **Technical Debt** - Refactoring needs are estimated

---

## 🏗️ Architecture

### Clean Architecture with Microservices

`	ext
🌐 WebAPI (Port 5000)          🤖 AIAssistant (Port 7276)
├── Controllers                 ├── gRPC Services
├── Application Services        ├── HuggingFace Integration
├── Domain Logic               ├── Google Gemini Integration
└── Infrastructure             └── Multi-Protocol Endpoints
    ├── Database (SQLite)          ├── Native gRPC
    └── Health Monitoring          └── gRPC-JSON Bridge
`

### Multi-Protocol Communication
`	ext
Frontend → WebAPI → AIAssistant
              ↓ (configurable)
              ├── HTTP/JSON ────→ REST API
              ├── gRPC ─────────→ Native gRPC
              └── gRPC-JSON ────→ JSON Bridge
`

### Planned Extensions
- **Event-Driven Architecture** with RabbitMQ
- **Domain Events** for loosely coupled services
- **CQRS Pattern** for read/write separation

---

## ✨ Features

- 🤖 **AI-Powered Analytics** - HuggingFace + Google Gemini for intelligent training analysis  
- 🔄 **Multi-Protocol API** - HTTP/REST, gRPC and gRPC-JSON Bridge
- 📊 **Training Plans** - Structured planning with progress tracking
- 🏥 **Health Monitoring** - Live dashboard with automatic service monitoring
- 🛡️ **Enterprise Error Handling** - Consistent exception management
- 🧪 **Architecture Testing** - Automatic Clean Architecture compliance

---

## 🔄 Multi-Protocol Communication

Three communication protocols for flexible microservice integration:

`ash
# HTTP/REST - Standard & Browser-compatible
POST http://localhost:7276/api/MotivationCoach/motivate

# Native gRPC - High Performance
grpc://localhost:7276/MotivationService/GetMotivation

# gRPC-JSON Bridge - Best of Both Worlds  
POST http://localhost:7276/grpc-json/MotivationService/GetMotivation
`

### Configuration
`json
{
  "AIAssistant": {
    "ClientType": "GrpcJson",    // "Http" | "Grpc" | "GrpcJson"
    "BaseUrl": "https://localhost:7276"
  }
}
`

| Protocol | Performance | Browser Support | Use Case |
|-----------|-------------|-----------------|----------|
| **HTTP/REST** | Standard | ✅ Full | Frontend, API Tools |
| **gRPC** | ⚡ Very Fast | ❌ Limited | Service-to-Service |
| **gRPC-JSON** | Standard | ✅ Full | Hybrid Integration |

---

## 🤖 AI-Powered Analytics

### AI Integration for Intelligent Training Analysis

- **Meta-Llama-3.1-8B-Instruct** via HuggingFace for fitness analysis
- **Google Gemini** for additional AI perspectives
- **Personalized Motivation** - Context-aware training tips
- **Workout Trends** - AI-based performance development
- **Robust Fallbacks** - Reliable operation at API limits

### Available Endpoints
`ash
# Workout Analysis
POST /api/WorkoutAnalysis/analyze/huggingface
POST /api/WorkoutAnalysis/analyze/googlegemini

# Motivation & Coaching
POST /api/MotivationCoach/motivate

# Multi-Protocol via gRPC-JSON Bridge
POST /grpc-json/MotivationService/GetMotivation
POST /grpc-json/WorkoutService/GetWorkoutAnalysis
`

---

## 🏥 Health Monitoring

Live monitoring of all services with automatic refresh:

- **/health-ui** - Visual dashboard with history  
- **/health** - JSON API for all services
- **Tag-based Grouping** - Services vs Infrastructure  
- **Auto-Refresh** - Every 60 seconds

`ash
# Open Health Dashboard
open http://localhost:8080/health-ui

# Check Health Status  
curl http://localhost:8080/health
`

---

## 🛠️ Tech Stack

**Backend:** .NET 8, Entity Framework Core, Clean Architecture  
**AI:** HuggingFace (Meta-Llama-3.1-8B), Google Gemini  
**Communication:** HTTP/REST, gRPC, gRPC-JSON Bridge  
**Database:** SQLite (Development), SQL Server (Production)  
**Quality:** xUnit, NetArchTest, SonarCloud, FluentAssertions  
**DevOps:** Docker, GitHub Actions, Health Monitoring  

**Planned:** RabbitMQ (Event-Driven)

---

## 🚀 Getting Started

### Docker (Recommended)
`ash
git clone https://github.com/your-org/SportMatrix.git
cd SportMatrix
docker-compose up
`

### Local Development
`ash
# Start API
cd SportMatrix.WebApi && dotnet run

# Start AI Service  
cd SportMatrix.AIAssistant && dotnet run
`

**Access:**
- Main API: https://localhost:5001
- AI Service: https://localhost:7276  
- Swagger UI: /swagger

---

## 🛡️ Error Handling

Consistent exception handling through Clean Architecture and Global Middleware.

### Exception Hierarchy
`	ext
Domain Exceptions
├── ActivityNotFoundException (404)
├── AthleteNotFoundException (404)
└── ValidationException (400)

Infrastructure Exceptions  
└── AIAssistantApiException (502)
`

### API Response Format
`json
{
  "type": "ActivityNotFound",
  "message": "Activity with ID 123 not found",
  "statusCode": 404,
  "timestamp": "2024-01-15T10:30:00Z"
}
`

**Principle:** Controllers are exception-free - Global Middleware handles all errors centrally.

---

## 📊 Data

The platform manages:
- Activity data (running, cycling, etc.)
- Performance metrics
- Routes and distances
- User profile data

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

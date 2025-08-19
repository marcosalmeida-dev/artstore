# ArtStore - Clean Architecture E-commerce Platform

A comprehensive .NET 9 e-commerce platform built with Clean Architecture principles, featuring CQRS pattern, multi-tenant support, and modern web technologies. This art store application demonstrates enterprise-level patterns and practices for building scalable, maintainable applications.

## 🏗️ Architecture Overview

This solution follows Clean Architecture principles with clear separation of concerns:

- **Domain Layer** (`ArtStore.Domain`): Core business entities, domain events, and business rules
- **Application Layer** (`ArtStore.Application`): Use cases, commands, queries, DTOs, and application services
- **Infrastructure Layer** (`ArtStore.Infrastructure`): Data access, external services, and cross-cutting concerns
- **Presentation Layer** (`ArtStore.UI`): Blazor Server UI with SignalR integration
- **Client Layer** (`ArtStore.UI.Client`): Blazor WebAssembly client application
- **Shared Layer** (`ArtStore.Shared`): Common DTOs and shared models

## ✨ Key Features

### Core Functionality
- **Product Management**: Complete product catalog with categories and detailed product information
- **Order Processing**: Full order lifecycle management with status tracking
- **Shopping Cart**: Interactive shopping cart with real-time updates
- **Payment Checkout**: Integrated payment processing workflow
- **User Authentication**: ASP.NET Core Identity with role-based access control
- **Multi-tenant Architecture**: Tenant isolation and data segregation

### Technical Features
- **CQRS Pattern**: Custom command/query handlers without external dependencies
- **Event-driven Architecture**: Domain events for loose coupling
- **Real-time Updates**: SignalR integration for live order management
- **Audit Trails**: Comprehensive tracking of data changes
- **Caching**: FusionCache integration for performance optimization
- **Internationalization**: Multi-language support with resource files
- **Export Functionality**: Excel and PDF export capabilities
- **Background Jobs**: Hangfire integration for task processing
- **API Documentation**: RESTful API with comprehensive endpoints

### UI/UX Features
- **Responsive Design**: MudBlazor component library for modern UI
- **Interactive Charts**: ApexCharts integration for data visualization
- **File Upload**: Support for document and image uploads
- **Hot Keys**: Keyboard shortcuts for improved productivity
- **Image Processing**: Advanced image handling and optimization
- **Search & Filtering**: Advanced product search and filtering capabilities

## 🛠️ Tech Stack

### Backend Technologies
- **.NET 9**: Latest version of the .NET framework
- **ASP.NET Core**: Web framework for building APIs and web applications
- **Entity Framework Core 9**: Object-relational mapping (ORM) framework
- **ASP.NET Core Identity**: Authentication and authorization framework
- **SignalR**: Real-time web functionality
- **Hangfire**: Background job processing
- **FusionCache**: High-performance caching library

### Frontend Technologies
- **Blazor Server**: Server-side Blazor for interactive web UI
- **Blazor WebAssembly**: Client-side Blazor for rich client applications
- **MudBlazor**: Material Design component library for Blazor
- **ApexCharts**: Modern charting library
- **Toolbelt.Blazor.HotKeys**: Keyboard shortcuts for Blazor

### Data & Storage
- **SQL Server**: Primary database (configurable for other providers)
- **Entity Framework Core**: Code-first approach with Fluent API
- **Data Protection**: ASP.NET Core Data Protection for key management

### Development & Deployment
- **Clean Architecture**: Domain-driven design patterns
- **CQRS**: Command Query Responsibility Segregation
- **Event Sourcing**: Domain event pattern implementation
- **Dependency Injection**: Built-in IoC container
- **Configuration**: Flexible configuration management
- **Logging**: Structured logging with Serilog

### Testing & Quality
- **Unit Testing**: Comprehensive test coverage structure
- **Integration Testing**: API and database integration tests
- **Code Analysis**: EditorConfig for consistent code style

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or Visual Studio Code

### Installation
1. Clone the repository
2. Update the connection string in `src/ArtStore.UI/appsettings.json`
3. Run the application - database will be created automatically
4. Navigate to the application URL to access the store

### Configuration
- Use the `X-Tenant-Code` header in API requests to specify the tenant
- Configure identity settings in `appsettings.json`
- Set up email settings for user notifications

## 📚 API Endpoints

### Orders Management
- `GET /api/orders` - Get orders with filtering and pagination
- `GET /api/orders/{id}` - Get specific order details
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}/status` - Update order status
- `GET /api/orders/export` - Export orders to Excel/PDF

### Products Management
- `GET /api/products` - Get product catalog with filtering
- `GET /api/products/{id}` - Get specific product details
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product information
- `DELETE /api/products/{id}` - Delete product

### Categories & Inventory
- `GET /api/categories` - Get product categories
- `GET /api/inventory` - Check product availability
- `POST /api/inventory/update` - Update inventory levels
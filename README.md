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
- **Product Management**: Complete product catalog with categories, detailed product information, and multi-image support
- **Order Processing**: Kanban-style order board with drag-and-drop status management and real-time updates
- **Shopping Cart**: Interactive shopping cart with quantity controls and real-time price calculations
- **Payment Checkout**: Integrated payment processing workflow with order summary and customer information
- **Coupon System**: Full coupon management with percentage, fixed amount, and free shipping discount types
- **Inventory Management**: Complete inventory tracking with locations, stock levels, safety stock, and reorder points
- **Bill of Materials (BOM)**: Recipe component management for finished products with raw material tracking
- **User Authentication**: ASP.NET Core Identity with role-based access control
- **Multi-tenant Architecture**: Tenant isolation and data segregation

### Advanced Inventory Features
- **Inventory Locations**: Manage multiple storage locations with location-specific stock tracking
- **Raw Material Tracking**: Mark products as raw materials and track them separately
- **Recipe Components**: Define Bill of Materials (BOM) for finished products with component quantities and units
- **Stock Level Monitoring**: Automatic low stock and below safety stock alerts
- **Advanced Filtering**: Search and filter inventory by location, stock levels, and product names

### Order Management Features
- **Kanban Board**: Visual drag-and-drop order management with status columns (Pending, Preparing, Ready, Completed, Cancelled)
- **Real-time Updates**: SignalR integration for live order status changes and automatic board updates
- **Order Sources**: Track order origin (POS, Online Store, Phone, Walk-in)
- **Status Workflow**: Streamlined status progression with action buttons for each stage
- **Order Details**: Comprehensive view of customer information, items, quantities, and order totals

### Coupon & Discount Features
- **Multiple Coupon Types**: Support for percentage discounts, fixed amount discounts, and free shipping
- **Usage Limits**: Set maximum usage limits per coupon with automatic tracking
- **Minimum Order Amount**: Configure minimum order requirements for coupon eligibility
- **Date Range Validation**: Start and end date controls with automatic expiration
- **Status Management**: Active/inactive toggle with visual status indicators

### Technical Features
- **CQRS Pattern**: Custom command/query handlers without external dependencies
- **Event-driven Architecture**: Domain events for loose coupling and reactive updates
- **Real-time Updates**: SignalR integration for live order management and notifications
- **Audit Trails**: Comprehensive tracking of data changes
- **Response Caching**: Output caching with performance optimization and compression
- **HybridCache**: High-performance distributed caching with local and distributed layers
- **Internationalization**: Multi-language support with resource files (English localization)
- **Export Functionality**: Excel and PDF export capabilities for orders and reports
- **Background Jobs**: Hangfire integration for task processing
- **API Documentation**: RESTful API with comprehensive endpoints

### UI/UX Features
- **Responsive Design**: MudBlazor component library for modern Material Design UI
- **Interactive Charts**: ApexCharts integration for data visualization
- **File Upload**: Support for document and product image uploads with multi-image management
- **Hot Keys**: Keyboard shortcuts for improved productivity
- **Image Processing**: Advanced image handling with primary image selection
- **Search & Filtering**: Advanced product, inventory, and coupon search with multiple filter criteria
- **Drag & Drop**: Intuitive Kanban board for order status management
- **Dialog Components**: Reusable modal dialogs for CRUD operations
- **Snackbar Notifications**: Real-time feedback for user actions
- **Animations**: Smooth transitions and visual feedback throughout the application

## 🛠️ Tech Stack

### Backend Technologies
- **.NET 9**: Latest version of the .NET framework
- **ASP.NET Core**: Web framework for building APIs and web applications
- **Entity Framework Core 9**: Object-relational mapping (ORM) framework
- **ASP.NET Core Identity**: Authentication and authorization framework
- **SignalR**: Real-time web functionality
- **Hangfire**: Background job processing
- **HybridCache**: High-performance distributed caching with two-tier architecture

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

## 📁 Project Structure

```
artstore/
├── src/
│   ├── ArtStore.Domain/              # Core domain entities and business logic
│   ├── ArtStore.Application/         # Application services, CQRS handlers
│   ├── ArtStore.Infrastructure/      # Data access, external services
│   ├── ArtStore.UI/                  # Blazor Server host application
│   ├── ArtStore.UI.Client/           # Blazor WebAssembly client
│   │   ├── Pages/
│   │   │   ├── Management/           # Admin management pages
│   │   │   │   ├── Order/            # Order management (Kanban board)
│   │   │   │   ├── Product/          # Product CRUD operations
│   │   │   │   ├── Category/         # Category management
│   │   │   │   ├── Coupon/           # Coupon management
│   │   │   │   └── Inventory/        # Inventory & BOM management
│   │   │   ├── Home.razor            # Main storefront
│   │   │   ├── Order.razor           # Customer order page
│   │   │   └── PaymentCheckout.razor # Checkout process
│   │   └── Services/                 # Client-side services
│   └── ArtStore.Shared/              # Shared DTOs and models
└── tests/                             # Test projects
```

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or Visual Studio Code
- Node.js (for client-side dependencies)

### Installation
1. Clone the repository
   ```bash
   git clone https://github.com/yourusername/artstore.git
   cd artstore
   ```

2. Update the connection string in `src/ArtStore.UI/appsettings.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ArtStoreDb;Trusted_Connection=True;"
   }
   ```

3. Run the application
   ```bash
   cd src/ArtStore.UI
   dotnet run
   ```
   Database will be created automatically on first run

4. Navigate to the application URL (typically `https://localhost:5001`)

### Configuration
- **Multi-tenancy**: Use the `X-Tenant-Code` header in API requests to specify the tenant
- **Identity**: Configure identity settings in `appsettings.json`
- **Email**: Set up email settings for user notifications
- **Caching**: HybridCache is configured with default expiration times for optimal performance
- **SignalR**: Real-time updates are enabled by default for order management

## 📚 API Endpoints

### Orders Management
- `GET /api/admin/orders` - Get orders with filtering and pagination
- `GET /api/admin/orders/{id}` - Get specific order details
- `POST /api/admin/orders` - Create new order
- `PUT /api/admin/orders/{id}/status` - Update order status
- `GET /api/admin/orders/export` - Export orders to Excel/PDF

### Products Management
- `GET /api/admin/products` - Get product catalog with filtering
- `GET /api/admin/products/{id}` - Get specific product details
- `POST /api/admin/products` - Create new product
- `PUT /api/admin/products/{id}` - Update product information
- `DELETE /api/admin/products/{id}` - Delete product
- `POST /api/admin/products/{id}/images` - Upload product images
- `PUT /api/admin/products/{id}/images/{imageId}/primary` - Set primary product image

### Categories Management
- `GET /api/admin/categories` - Get product categories
- `GET /api/admin/categories/{id}` - Get specific category
- `POST /api/admin/categories` - Create new category
- `PUT /api/admin/categories/{id}` - Update category
- `DELETE /api/admin/categories/{id}` - Delete category

### Coupon Management
- `GET /api/admin/coupons` - Get all coupons with filtering
- `GET /api/admin/coupons/{id}` - Get specific coupon details
- `POST /api/admin/coupons` - Create new coupon
- `PUT /api/admin/coupons/{id}` - Update coupon
- `DELETE /api/admin/coupons/{id}` - Delete coupon
- `POST /api/admin/coupons/validate` - Validate coupon code

### Inventory Management
- `GET /api/admin/inventorylocations` - Get all inventory locations
- `POST /api/admin/inventorylocations` - Create new inventory location
- `PUT /api/admin/inventorylocations/{id}` - Update inventory location
- `DELETE /api/admin/inventorylocations/{id}` - Delete inventory location

### Inventory Items
- `GET /api/admin/inventoryitems` - Get all inventory items with stock levels
- `GET /api/admin/inventoryitems/{id}` - Get specific inventory item
- `POST /api/admin/inventoryitems` - Create new inventory item
- `PUT /api/admin/inventoryitems/{id}` - Update inventory item
- `DELETE /api/admin/inventoryitems/{id}` - Delete inventory item

### Recipe Components (BOM)
- `GET /api/admin/recipecomponents` - Get all recipe components
- `GET /api/admin/recipecomponents/{id}` - Get specific recipe component
- `POST /api/admin/recipecomponents` - Create new recipe component
- `PUT /api/admin/recipecomponents/{id}` - Update recipe component
- `DELETE /api/admin/recipecomponents/{id}` - Delete recipe component

## 🆕 Recent Enhancements

### Latest Features (2025)
- **Kanban Order Board**: Refactored order management into an intuitive drag-and-drop Kanban board with real-time SignalR updates
- **Inventory Management System**: Complete inventory tracking with locations, safety stock levels, and BOM support
- **Bill of Materials (BOM)**: Recipe component system for managing raw materials and finished products
- **Coupon Management**: Full-featured coupon system with multiple discount types and validation
- **Performance Optimizations**:
  - Response compression and output caching for API endpoints
  - HybridCache integration for improved data access performance with two-tier caching
  - Optimized product image management with primary image selection
- **Enhanced UI/UX**:
  - Improved order status workflow with visual indicators
  - Multi-image product management
  - Raw material product categorization
  - Advanced filtering and search across all management interfaces
- **Code Quality Improvements**:
  - Refactored navigation and routing structure
  - Enhanced event handling for order creation and updates
  - Localization improvements (English)
  - Streamlined dependency injection and service configuration

### Key Improvements
- **Real-time Updates**: SignalR integration ensures all users see order status changes immediately
- **Stock Management**: Low stock and below safety stock alerts help maintain inventory levels
- **Order Workflow**: Simplified order status progression with single-click actions
- **Discount System**: Flexible coupon system supporting various business scenarios
- **Developer Experience**: Improved code organization and cleaner architecture

## 🔄 SignalR Hub Endpoints

The application uses SignalR for real-time communication:

### OrderHub (`/hubs/orders`)
- **Events Received**: Order created, status updated, order deleted
- **Events Broadcast**: Automatic updates to all connected clients
- **Use Case**: Real-time order board synchronization across multiple users

## 🎯 Usage Scenarios

### For Restaurant/Food Service
- Track orders from multiple sources (POS, online, phone)
- Manage raw materials and finished products with BOM
- Monitor inventory levels and reorder points
- Visual Kanban board for kitchen order management

### For E-commerce
- Complete product catalog management
- Coupon and discount management
- Order processing and fulfillment tracking
- Multi-location inventory management

### For Retail
- Point of sale integration ready
- Inventory tracking across multiple locations
- Product variant management
- Customer order history and tracking

## 📝 License

This project is available for educational and demonstration purposes.
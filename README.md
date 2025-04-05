
# 🛒 Supermarket Management System

A full-featured web application designed to manage clients, products, and billing for supermarkets. Built with **ASP.NET Core MVC** for the frontend and **ASP.NET Core Web API** for the backend using a **code-first approach** with **Entity Framework Core**.

---

## 🚀 Features Overview

### ✅ General Features

- Clean and intuitive UI using Bootstrap 5
- Sidebar navigation with active page highlighting
- Fully responsive layout with collapsible sidebar
- Avanced filtering, and pagination for smooth navigation and data management
- Error Handling: Comprehensive error handling for API requests and UI operations.

---

## 📂 Project Structure

```bash
SupermarketManagement/
│
├── SM_API/                  # Backend API
│   ├── Controllers/         # API controllers (Clients, Products, Bills)
│   ├── DTOs/                # Data Transfer Objects for clean API communication
│   ├── Models/              # Entity models
│   ├── Data/                # DbContext and configuration
│   ├── Services/            # Business logic services
│   └── Program.cs           # API startup configuration
│
├── SM_Web/                  # Frontend MVC Application
│   ├── Controllers/         # MVC controllers (Clients, Products, Bills)
│   ├── Views/               # Razor Views (Shared, Clients, Products, Bills)
│   ├── wwwroot/             # Static files (CSS, JS, images)
│   ├── Models/              # ViewModels for UI binding
│   ├── Services/            # ApiService to communicate with SM_API
│   ├── Layout/              # Shared layout (_Layout.cshtml)
│   └── Program.cs           # Web startup configuration
│
├── README.md
└── SupermarketManagement.sln
```

---

## 🌐 SM_API - Backend (RESTful API)

### Technologies:
- ASP.NET Core Web API
- Entity Framework Core (Code-First)
- AutoMapper
- Fluent Validation
- Swagger UI for API testing

### Core Endpoints

#### Clients API
- `GET /api/clients` - List all clients
- `GET /api/clients/{id}` - Get single client by ID
- `POST /api/clients` - Create a new client
- `PUT /api/clients/{id}` - Update client info
- `DELETE /api/clients/{id}` - Delete a client

#### Products API
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get single product
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

#### Bills API
- `GET /api/bills` - List all bills
- `GET /api/bills/{id}` - Bill details
- `POST /api/bills` - Create a bill  
  > Updates client balance and adjusts product stock  
- `DELETE /api/bills/{id}` - Delete bill  
  > Reverts client balance and restores product stock

### Key Logic in API

- When creating a bill:
  - Total is calculated from selected products.
  - Client's balance is increased.
  - Product stock quantities are decreased accordingly.
- When deleting a bill:
  - Reverse operations are applied to restore data integrity.

---

## 🖥️ SM_Web - Frontend (ASP.NET Core MVC)

### Technologies:
- ASP.NET Core MVC
- Bootstrap 5 + jQuery
- Razor Views + Layout system
- Select2 (for searchable dropdowns)
- Font Awesome (icons)

### UI Pages

#### 🧑 Clients
- List all clients with pagination & filtering
- Add/Edit/Delete client

#### 📦 Products
- Display products with code, stock, and price
- Pagination & filtering
- Add/Edit/Delete product

#### 🧾 Bills
- Create new bills with searchable product dropdown or scan by code
- Adjusts client balance and stock in real time
- View bill list with filters (client, date, phone, bill type)
- Delete bills and rollback changes

### 🔁 API Integration
The web app uses a central `ApiService` to communicate with `SM_API` using `HttpClient`. Each controller in the web app calls `ApiService` methods for CRUD operations.

---

## 📦 Setup Instructions

### Prerequisites
- .NET 8 SDK or later
- SQL Server
- Visual Studio 2022 or VS Code

### Run the Project

1. **Clone the repository:**

   ```bash
   git clone https://github.com/mehofwzy/SupermarketManagementSystem.git
   ```

2. **Setup the Database:**
   - Open `SM_API/appsettings.json`
   - Configure your SQL Server connection string.

3. **Run Migrations:**

   ```bash
   cd SM_API
   dotnet ef database update
   ```

4. **Run the Projects:**

   - Run `SM_API` first
   - Then run `SM_Web`

5. Visit:  
   - API: `https://localhost:5001/swagger`  
   - Web: `https://localhost:5002`

---

## 📜 License

This project is made by Eng.Mohamed Fawzy - .NET Software Developer

---

## 👨‍💻 Author

Developed by **Mohamed Fawzy**

- 📧 Email: [mehofawzy@outlook.com](mailto\:mehofawzy@outlook.com)
- 📞 Phone: [+201095194149]
- 🌐 linkedIn: [linkedin.com/in/mehofwzy](https://linkedin.com/in/mehofwzy)

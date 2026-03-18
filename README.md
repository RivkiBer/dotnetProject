# 🥐 MyBakery - API System

## 📋 Overview
**MyBakery** הוא מערכת ניהול מקיפה למאפיה , בנויה על ASP.NET Core עם עיצוב RESTful API מודרני וממשק משתמש אינטראקטיבי.

---

## 👥 Team
- **Rivki Berenfeld** 👩‍💼
- **Shevi Shapira** 👩‍💼

---

## ✨ Features

### 🔐 Authentication & Security
- ✅ Secure user registration and login
- ✅ Password encryption using BCrypt
- ✅ JWT Token-based authentication
- ✅ User session management

### 🛒 Core Functionality
- 📦 Complete Bakery inventory management
- 👤 User profile management
- 🔔 Real-time notifications via WebSockets
- 📊 Activity logging and monitoring

### 🎨 User Interface
- 💻 Modern, responsive web interface
- 🌐 Real-time updates with SignalR
- 📱 User-friendly dashboard
- 🎯 Intuitive product browsing

---

## 🏗️ Architecture

### Project Structure
```
MyBakery/
├── Controllers/          # API endpoints
│   ├── BakaryControler.cs
│   ├── LoginController.cs
│   └── UserController.cs
├── Services/             # Business logic
│   ├── BakeryService.cs
│   ├── UserService.cs
│   ├── ActiveUserService.cs
│   ├── TokenService.cs
│   └── LoggingBackgroundService.cs
├── Models/              # Data models
│   ├── User.cs
│   ├── Pastry.cs
│   ├── UserDto.cs
│   └── LogMessage.cs
├── Hubs/                # WebSocket endpoints
│   └── ItemsHub.cs
├── Middleware/          # Custom middleware
│   ├── ExceptionHandlingMiddleware.cs
│   ├── My1stMiddleware.cs
│   └── MyLogMiddleware.cs
├── Data/                # Data persistence
│   ├── Users.json
│   └── Pastries.json
└── wwwroot/             # Frontend assets
    ├── index.html
    ├── login.html
    ├── user.html
    └── css/, js/
```

---

## 🚀 Getting Started

### Prerequisites
- 🔵 .NET 9.0 SDK or higher
- ✅ Visual Studio / VS Code
- 🗄️ JSON-based data storage (included)

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd dotnetProject
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Build the project**
```bash
dotnet build
```

4. **Run the application**
```bash
dotnet run MyBakery.csproj
```

5. **Access the application**
- 🌐 Open your browser and navigate to `https://localhost:5001`
- 📝 Login page available at `/login.html`
- 👤 User dashboard at `/user.html`

---

## 📡 API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/login` | User login |
| POST | `/api/register` | New user registration |
| POST | `/api/logout` | User logout |

### Bakery Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/bakery` | Get all products |
| GET | `/api/bakery/{id}` | Get product by ID |
| POST | `/api/bakery` | Add new product |
| PUT | `/api/bakery/{id}` | Update product |
| DELETE | `/api/bakery/{id}` | Delete product |

### User Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/user` | Get user profile |
| GET | `/api/user/{id}` | Get user by ID |
| PUT | `/api/user/{id}` | Update user |
| DELETE | `/api/user/{id}` | Delete user |

### Real-time Notifications
- 🔔 WebSocket Hub: `/itemsHub`
- ✅ Live inventory updates
- ✅ User activity notifications

---

## 🔧 Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Database
- 📁 Data stored in `Data/` folder as JSON files
- 📄 `Users.json` - User information
- 📄 `Pastries.json` - Product inventory

---

## 🛠️ Technologies Used

| Technology | Purpose |
|-----------|---------|
| **ASP.NET Core 9** | Web framework |
| **C#** | Backend language |
| **SignalR** | Real-time communication |
| **BCrypt** | Password hashing |
| **JSON** | Data storage |
| **HTML/CSS/JS** | Frontend |

---

## 📝 Middleware

### Custom Middleware Pipeline
1. **ExceptionHandlingMiddleware** - Global exception handling
2. **MyLogMiddleware** - HTTP request/response logging
3. **My1stMiddleware** - Custom request processing

---

## 🔍 Logging

### Log Files
- 📂 Location: `Logs/` folder
- 📋 Format: `logs_YYYY-MM-DD.txt`
- 🔄 Automatic daily rotation
- ⏰ Includes timestamps and request details

---

## 🎯 Key Services

### BakeryService
- Manages bakery product inventory
- CRUD operations for pastries
- Real-time inventory synchronization

### UserService
- User registration and management
- Secure password handling
- User profile updates

### ActiveUserService
- Tracks active user sessions
- Real-time user status
- Notification broadcasting

### TokenService
- JWT token generation
- Token validation
- Session management

---

## 🚨 Error Handling

The application includes comprehensive error handling:
- ✅ Global exception middleware
- ✅ Validation error messages
- ✅ HTTP status code responses
- ✅ Detailed logging of errors

---

## 📊 Development Status

| Feature | Status |
|---------|--------|
| User Authentication | ✅ Complete |
| Product Management | ✅ Complete |
| Real-time Updates | ✅ Complete |
| Logging System | ✅ Complete |
| WebUI | ✅ Complete |

---

## 📞 Support

For issues or questions:
1. 📧 Check existing issues
2. 📝 Review documentation
3. 💬 Contact the development team

---

## 📄 License

This project is developed for educational and commercial purposes.

---

## 🎉 Credits

**Developed by:**
- **Rivka Barnfeld** 👩‍💻
- **Shevi Spira** 👩‍💻

---

<div align="center">

### 🥐 MyBakery - Making bakery management sweet! 🥐

**Last Updated:** March 2026

</div>

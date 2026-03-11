# Stadium Management System

A comprehensive ASP.NET Core web application for managing stadium operations, match schedules, and ticket bookings.

## Overview

The Stadium Management System is built with **ASP.NET Core 8.0** and provides a complete solution for:
- Stadium match and event management
- Ticket booking and management
- User and staff role-based access
- Email notifications and OTP verification
- PDF ticket generation
- User authentication and account management

## Features

### For Users
- Browse available matches and events
- Book and purchase tickets
- View booking history
- Generate PDF tickets
- Manage account and password
- Email and OTP-based verification

### For Staff
- Manage matches and schedules
- View and manage ticket sales
- Monitor user accounts
- Dashboard with key statistics
- Secure staff authentication

### General Features
- Session-based authentication (30-minute timeout)
- Email notifications via SMTP (Gmail)
- OTP verification for secure operations
- PDF ticket generation
- Responsive web interface
- Role-based access control

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Architecture**: MVC (Model-View-Controller)

### Database
- **Database**: MySQL
- **Driver**: MySql.Data (v8.3.0)

### Libraries & Services
- **Email Service**: MailKit (v4.3.0)
- **PDF Generation**: iText7 (v9.5.0)
- **SMS/OTP**: Twilio (v7.14.3)
- **Security**: Bouncy Castle Adapter (v9.5.0)

## Project Structure

```
stadium_output/
├── Controllers/              # MVC Controllers
│   ├── HomeController.cs
│   ├── StaffController.cs
│   └── UserController.cs
├── Views/                    # Razor Templates
│   ├── Home/
│   ├── Staff/
│   ├── User/
│   └── Shared/              # Layout & common views
├── Models/                   # Data Models
│   └── Models.cs
├── Data/                     # Data Layer
│   └── DbHelper.cs
├── Services/                 # Business Logic
│   ├── EmailService.cs
│   ├── OtpService.cs
│   └── TicketPdfService.cs
├── Helpers/                  # Utility Classes
│   └── IplSvgHelper.cs
├── wwwroot/                  # Static Files
│   └── css/
│       └── site.css
├── appsettings.json          # Configuration
├── Program.cs                # Application Startup
└── StadiumWeb.csproj         # Project File
```

## Prerequisites

- **.NET 8.0 SDK** or later
- **MySQL Server** (for database)
- **Email Account** (Gmail recommended for SMTP)
- **Twilio Account** (for OTP/SMS services, optional)

## Setup Instructions

### 1. Database Setup

1. Create a MySQL database named `stadium_demo`:
   ```sql
   CREATE DATABASE stadium_demo;
   ```

2. Run the database initialization script:
   ```bash
   mysql -u root -p stadium_demo < stadium_demo.sql
   ```

3. Verify the connection in `appsettings.json`

### 2. Configuration

Update `appsettings.json` with your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=stadium_demo;uid=root;pwd=YOUR_PASSWORD;"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromName": "Stadium Name"
  }
}
```

**Note**: For Gmail, use an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password.

### 3. Build and Run

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The application will be available at `https://localhost:5001`

## Usage

### For Users
1. Navigate to the home page
2. Click "Register" to create an account
3. Verify your email with the OTP
4. Browse available matches
5. Book tickets
6. Download PDF tickets from booking history

### For Staff
1. Use staff login page
2. Access dashboard with management features
3. Add new matches
4. Monitor ticket sales and user information

## Default Routes

| Route | Controller | Action | Purpose |
|-------|-----------|--------|---------|
| `/` | Home | Index | Home page |
| `/user/register` | User | Register | User registration |
| `/user/login` | User | Login | User login |
| `/user/dashboard` | User | Dashboard | User dashboard |
| `/staff/login` | Staff | Login | Staff login |
| `/staff/dashboard` | Staff | Dashboard | Staff dashboard |

## Configuration Details

### Session Configuration
- **Idle Timeout**: 30 minutes
- **HttpOnly Cookies**: Enabled (security)
- **Essential Cookies**: Enabled

### Services Registered
- `DbHelper`: Database operations
- `TicketPdfService`: PDF ticket generation
- `OtpService`: OTP management
- `EmailService`: Email notifications

## Security Features

- Session-based authentication
- HttpOnly cookies to prevent XSS attacks
- OTP verification for sensitive operations
- Password hashing and encryption
- Role-based access control (Staff vs User)

## Dependencies

Install required NuGet packages:
```bash
dotnet add package MySql.Data --version 8.3.0
dotnet add package MailKit --version 4.3.0
dotnet add package itext7 --version 9.5.0
dotnet add package itext7.bouncy-castle-adapter --version 9.5.0
dotnet add package Twilio --version 7.14.3
```

## Troubleshooting

### Database Connection Issues
- Verify MySQL server is running
- Check connection string in `appsettings.json`
- Ensure database `stadium_demo` exists

### Email Service Not Working
- Verify SMTP credentials in `appsettings.json`
- For Gmail, use App Password (not regular password)
- Check if 2FA is enabled on Gmail account

### Session Timeout
- Default timeout is 30 minutes of inactivity
- User will be logged out and redirected to login page

## Future Enhancements

- Payment gateway integration
- Real-time ticket availability
- Advanced reporting and analytics
- Mobile app support
- Multi-language support
- Seat selection interface

## License

This project is developed as part of Semester 5 coursework.

## Contact & Support

For issues or questions, please contact the development team.

---

**Last Updated**: March 2026

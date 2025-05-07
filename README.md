# Blog System with Role-Based Authentication

A feature-rich blog system built with ASP.NET Core MVC, featuring role-based authentication, blog post management, and a commenting system. This project demonstrates best practices in ASP.NET Core development, including clean architecture, dependency injection, and secure authentication.

## Features

### User Authentication & Role Management
- User registration and login with email confirmation
- Role-based access control (Admin, Editor, Reader)
- User profile management with avatar support
- Secure password handling with ASP.NET Core Identity
- Two-factor authentication support
- Account lockout after failed attempts

### Blog Post Management
- Create, edit, and delete blog posts with rich text editor
- Categorize posts with hierarchical categories
- Tag system with auto-complete
- Post status management (Draft, Published, Archived)
- Featured posts functionality
- Post scheduling for future publication
- Post version history

### Commenting System
- Comment on blog posts with markdown support
- Nested replies with threading
- Comment moderation queue
- Comment approval system
- Spam protection
- Email notifications for replies

### Search & Filtering
- Full-text search across posts and comments
- Advanced filtering by categories and tags
- Sort by date, popularity, and relevance
- Search suggestions
- Search history

## Technology Stack

- ASP.NET Core 7.0 MVC
- Entity Framework Core 7.0
- ASP.NET Core Identity 7.0
- SQL Server 2019
- Bootstrap 5.3.0
- jQuery 3.6.0
- TinyMCE 6.0 (Rich Text Editor)
- Markdig (Markdown Processing)
- AutoMapper 12.0
- FluentValidation 11.0

## Database Schema

The system uses the following main entities:
- Users (AspNetUsers)
- Roles (AspNetRoles)
- BlogPosts
- Categories
- Tags
- Comments
- PostTags (Many-to-Many relationship)

## Prerequisites

- .NET 7.0 SDK or later
- SQL Server 2019 or later (LocalDB or full version)
- Visual Studio 2022 or later (recommended)
- Git

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/yourusername/blog-system.git
```

2. Navigate to the project directory:
```bash
cd blog-system
```

3. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=BlogSystem;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

4. Install required NuGet packages:
```bash
dotnet restore
```

5. Run the following commands to set up the database:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

6. Run the application:
```bash
dotnet run
```

7. Access the application at `https://localhost:5001` or `http://localhost:5000`

## Default Admin Account

The system creates a default admin account during initialization:
- Email: admin@blog.com
- Password: Admin123!

## Role-Based Access

### Admin
- Full system access
- Manage users and roles
- Create, edit, and delete posts
- Moderate comments
- Delete any content
- Access to system settings
- View analytics and reports

### Editor
- Create and edit posts
- Cannot delete posts
- Moderate comments
- Cannot manage users
- Access to post analytics
- Manage categories and tags

### Reader
- View posts
- Leave comments
- No content management rights
- Subscribe to categories
- Save favorite posts

## API Endpoints

### Authentication
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/logout
- POST /api/auth/refresh-token

### Blog Posts
- GET /api/posts
- GET /api/posts/{id}
- POST /api/posts
- PUT /api/posts/{id}
- DELETE /api/posts/{id}

### Comments
- GET /api/posts/{postId}/comments
- POST /api/posts/{postId}/comments
- PUT /api/comments/{id}
- DELETE /api/comments/{id}

## Troubleshooting

### Common Issues

1. Database Connection Issues
   - Verify SQL Server is running
   - Check connection string in appsettings.json
   - Ensure user has proper permissions

2. Migration Errors
   - Delete existing migrations folder
   - Run `dotnet ef database drop`
   - Run migrations again

3. Build Errors
   - Clean solution
   - Delete bin and obj folders
   - Restore NuGet packages

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- ASP.NET Core team for the excellent framework
- Bootstrap team for the UI components
- All contributors who have helped improve this project 
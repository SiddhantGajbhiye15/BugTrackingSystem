# 🐞 Bug Tracking System

<div align="center">

A full-stack, role-based Bug Tracking System built with  
**ASP.NET Core Web API, Entity Framework Core, SQL Server, React and Tailwind CSS**

Designed to model a real software team's workflow from  
**bug reporting → assignment → development → testing → closure**.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=black)](https://react.dev/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Vite](https://img.shields.io/badge/Vite-Frontend-646CFF?logo=vite)](https://vite.dev/)
[![JWT](https://img.shields.io/badge/Auth-JWT-black?logo=jsonwebtokens)](https://jwt.io/)

### 🚀 [Live Application](https://bug-tracking-system-ten.vercel.app/login)

</div>

---

## 📌 Overview

The **Bug Tracking System** is a full-stack web application designed to simulate how software development teams manage projects, team members, bugs and bug-resolution workflows.

Unlike a basic CRUD application, the system contains role-based permissions and business rules that control what each user can do.

The application supports four roles:

- Admin
- Project Manager
- Developer
- Tester

Each role receives its own dashboard, permissions and workflow.

---

## 🚀 Live Demo

### Frontend

https://bug-tracking-system-ten.vercel.app/login

### Backend API

https://bugtrackingsiddhant.runasp.net

> The backend root URL does not contain a public page. The React frontend communicates with the backend API.

---

# 🔐 Demo Accounts

The following accounts are available for recruiters and reviewers to test the deployed application.

| Role | Email | Password |
|---|---|---|
| Project Manager | `chetanbonsule@gmail.com` | `Chetan@123` |
| Tester | `partharamarker@tester.com` | `Parth@123` |
| Developer | `manish@test.com` | `Manish@123` |
| Developer | `pranav@gmail.com` | `Pranay@123` |

> These are shared demo accounts created only for testing. Data inside the deployed demo environment may change as different users test the application.

---

# 🎯 Recommended Demo Flow

The easiest way to understand the system is to follow the complete bug lifecycle.

### 1. Login as Project Manager

Use:

```text
chetanbonsule@gmail.com
Chetan@123
```

The Project Manager can:

- create projects
- manage project members
- view project bugs
- assign bugs
- reassign bugs
- manage project lifecycle

### 2. Login as Tester

Use:

```text
partharamarker@tester.com
Parth@123
```

Create a bug and provide:

- title
- description
- bug type
- priority
- expected output
- actual output
- steps to reproduce
- optional evidence link

### 3. Login again as Project Manager

Assign the reported bug to a Developer.

### 4. Login as Developer

Use:

```text
manish@test.com
Manish@123
```

Move the assigned bug through:

```text
Assigned
   ↓
In Progress
   ↓
Resolved
```

### 5. Login as Tester

Verify the resolved bug.

If the fix works:

```text
Resolved → Closed
```

If the issue still exists:

```text
Resolved → Reopened
```

---

# ✨ Core Features

## Authentication & Security

- JWT-based authentication
- Role-based authorization
- Protected ASP.NET Core API endpoints
- Protected React routes
- Password hashing using BCrypt
- Automatic JWT attachment through Axios interceptors
- Automatic logout when authentication expires
- User activation and deactivation
- Secure Admin bootstrap/seeding support
- Backend validation of permissions and business rules

---

## 👥 User Management

Admin users can:

- create users
- view all users
- view individual users
- edit user information
- activate users
- deactivate users
- reset user passwords
- view project information
- change a project's Project Manager

Authenticated users can:

- view their own profile
- change their own password

---

# 👤 Roles & Permissions

## Admin

The Admin manages organization-level users and administration.

### Permissions

- Create users
- Edit users
- Activate users
- Deactivate users
- Reset passwords
- View users
- View projects
- Change Project Managers

The Admin does not participate directly in the bug-development workflow.

---

## Project Manager

The Project Manager controls project execution.

### Permissions

- Create projects
- Edit projects
- View managed projects
- Manage project members
- Add Developers
- Add Testers
- Remove active members
- View available employees
- View project bugs
- Filter bugs
- Assign bugs to Developers
- Reassign bugs
- Change bug priority according to workflow rules
- View Developer workload
- Complete projects
- Archive completed projects
- Restore archived projects
- Delete only unused projects

---

## Developer

Developers work on bugs assigned to them.

### Permissions

- View assigned bugs
- View bug details
- Add comments
- Edit/delete their own comments
- Start working on bugs
- Resolve bugs

Developer workflow:

```text
Assigned
   ↓
InProgress
   ↓
Resolved
```

Developers cannot directly close bugs.

---

## Tester

Testers report bugs and verify fixes.

### Permissions

- Create bugs
- View project bugs
- View bug details
- Update bugs according to workflow rules
- Delete bugs according to workflow rules
- Add comments
- Edit/delete their own comments
- Verify resolved bugs
- Close successfully fixed bugs
- Reopen bugs that failed verification

---

# 🐞 Bug Lifecycle

The application implements a controlled bug lifecycle.

```text
                    ┌──────────────┐
                    │ Bug Reported │
                    └──────┬───────┘
                           │
                           ▼
                        Open
                           │
                           │ Project Manager
                           │ assigns Developer
                           ▼
                       Assigned
                           │
                           │ Developer starts work
                           ▼
                      InProgress
                           │
                           │ Developer finishes fix
                           ▼
                       Resolved
                        /      \
                       /        \
              Tester verifies   Tester rejects fix
                     /            \
                    ▼              ▼
                 Closed        Reopened
                                  │
                                  └── continues through
                                      the workflow
```

---

# ⚙️ Important Business Rules

The application intentionally enforces business rules on the **backend**, not only in the user interface.

### Project Membership

A Developer or Tester can belong to only:

```text
ONE active project at a time
```

If a user already belongs to another active project, that user is not available for assignment.

---

### Bug Assignment

Only a Project Manager can assign or reassign a bug.

The assigned Developer must belong to that project.

---

### Developer Status Changes

Developers can perform only their allowed workflow transitions:

```text
Assigned → InProgress → Resolved
```

A Developer cannot directly close a bug.

---

### Tester Verification

A Tester is responsible for verification after the Developer resolves the issue.

```text
Resolved → Closed
```

or

```text
Resolved → Reopened
```

---

### Bug Priority

Priorities supported by the system:

```text
Low
Medium
High
Critical
```

Priority management follows the application's workflow restrictions.

---

### Bug Types

Supported bug categories:

```text
UI
Functional
Performance
Security
Other
```

---

# 📁 Project Lifecycle

Projects follow a controlled lifecycle.

```text
Active
   │
   │ Complete
   ▼
Completed
   │
   │ Archive
   ▼
Archived
   │
   │ Restore
   ▼
Active
```

---

## Project Completion Rule

A project cannot be marked as completed while unfinished bugs exist.

The backend checks whether any bug is not `Closed`.

Therefore:

```text
Open        ❌
Assigned    ❌
InProgress  ❌
Resolved    ❌
Reopened    ❌

Closed      ✅
```

All bugs must be closed before the project can be completed.

---

## Project Archiving

When a completed project is archived:

- project status changes to `Archived`
- active member assignments are ended
- `RemovedDate` is recorded
- users become available for another project
- project records remain stored
- bugs remain stored
- comments remain stored
- historical membership records remain stored

Archiving therefore preserves project history instead of permanently removing it.

---

## Project Restore

An archived project can be restored:

```text
Archived → Active
```

Previously assigned members are **not automatically reactivated**.

This prevents conflicts if those employees have already been assigned to another active project.

The Project Manager can choose new available members after restoration.

---

## Project Deletion

Permanent deletion is intentionally restricted.

Deletion is allowed only when the project has never accumulated real project history.

If a project contains:

```text
Project Members
OR
Historical Project Members
OR
Bugs
```

permanent deletion is blocked.

Real projects should be **archived**, not deleted.

---

# 📊 Role-Based Dashboards

The application provides different dashboards based on the authenticated user's role.

## Admin Dashboard

Includes:

- user statistics
- active/inactive user information
- employee listing
- user roles
- user management actions
- project overview

---

## Project Manager Dashboard

Includes:

- managed projects
- project information
- active member counts
- bug statistics
- open bugs
- critical bugs
- Developer workload
- project management actions

---

## Developer Dashboard

Includes:

- assigned project information
- assigned bugs
- bugs currently being worked on
- status information
- bug workflow controls

---

## Tester Dashboard

Includes:

- assigned project
- reported bugs
- project bugs
- bug status information
- bug verification workflow

---

# 💬 Bug Comments

Project participants can communicate through bug-specific comments.

Supported functionality includes:

- create comments
- view comments
- edit own comments
- delete own comments

Comments remain attached to their corresponding bug.

---

# 🔍 Search & Filtering

The application supports searching and filtering across project and bug data.

Bug filtering includes:

- status
- priority
- assigned Developer

Project screens also support search functionality.

---

# 🧰 Technology Stack

## Frontend

| Technology | Purpose |
|---|---|
| React 19 | User interface |
| JavaScript | Frontend application logic |
| React Router | Client-side routing |
| Axios | HTTP/API communication |
| Tailwind CSS | Styling |
| Lucide React | Icons |
| Vite | Development/build tooling |

---

## Backend

| Technology | Purpose |
|---|---|
| C# | Backend language |
| .NET 10 | Application runtime |
| ASP.NET Core Web API | REST API |
| Entity Framework Core 10 | ORM |
| LINQ | Data querying |
| JWT Bearer Authentication | Authentication |
| BCrypt | Password hashing |
| Swagger / OpenAPI | Development API documentation |

---

## Database

| Technology | Purpose |
|---|---|
| Microsoft SQL Server | Relational database |
| EF Core Code First | Database modelling |
| EF Core Migrations | Schema versioning |

---

## Deployment

| Component | Platform |
|---|---|
| Frontend | Vercel |
| Backend | ASP.NET Hosting |
| Database | SQL Server |

---

# 🏗️ Application Architecture

The backend follows a layered architecture.

```text
┌──────────────────────────┐
│       React Frontend     │
│                         │
│ React + Axios + Router  │
└────────────┬─────────────┘
             │
             │ HTTP / JSON
             ▼
┌──────────────────────────┐
│     ASP.NET Controllers  │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│        Services          │
│                         │
│ Business Rules          │
│ Authorization Logic     │
│ Workflow Validation     │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│       Repositories       │
│                         │
│ Data Access Logic       │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│   Entity Framework Core │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│       SQL Server         │
└──────────────────────────┘
```

---

# 🔄 Example Request Flow

Example: a Project Manager assigns a bug.

```text
User clicks "Assign"
        ↓
React Component
        ↓
Axios PATCH Request
        ↓
ASP.NET Core Controller
        ↓
Service Layer
        ↓
Business Rule Validation
        ↓
Repository
        ↓
Entity Framework Core
        ↓
SQL Server
        ↓
API Response
        ↓
React State Updated
        ↓
Updated UI
```

---

# 🗃️ Database Model

Main entities:

```text
User
Project
ProjectMember
Bug
Comment
```

High-level relationships:

```text
User
 ├── ProjectMemberships
 ├── Reported Bugs
 ├── Assigned Bugs
 └── Comments

Project
 ├── ProjectMembers
 └── Bugs

Bug
 └── Comments
```

---

# 📂 Project Structure

```text
BugTrackingSystem/
│
├── backend/
│   ├── BugTrackingSystem.slnx
│   │
│   └── BugTrackingSystem/
│       ├── Configurations/
│       ├── Controllers/
│       ├── DTOs/
│       ├── Data/
│       ├── Entities/
│       ├── Enums/
│       ├── Exceptions/
│       ├── Helpers/
│       ├── Interfaces/
│       ├── Migrations/
│       ├── Repositories/
│       ├── Services/
│       ├── Properties/
│       ├── Program.cs
│       └── BugTrackingSystem.csproj
│
├── frontend/
│   ├── src/
│   │   ├── api/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── App.jsx
│   │   └── main.jsx
│   │
│   ├── package.json
│   ├── vite.config.js
│   └── vercel.json
│
├── documents/
├── .gitignore
└── README.md
```

---

# 🌐 API Overview

The backend exposes REST APIs protected through JWT and role-based authorization.

## Authentication

```http
POST /api/Auth/login
```

---

## Users

```http
POST  /api/Users
GET   /api/Users
GET   /api/Users/{id}
PUT   /api/Users/{id}

PATCH /api/Users/{id}/activate
PATCH /api/Users/{id}/deactivate
PATCH /api/Users/{id}/reset-password

GET   /api/Users/me
PUT   /api/Users/change-password
```

---

## Projects

```http
POST   /api/Projects
GET    /api/Projects
GET    /api/Projects/{id}
PUT    /api/Projects/{id}
DELETE /api/Projects/{id}

PATCH /api/Projects/{id}/manager
PATCH /api/Projects/{id}/complete
PATCH /api/Projects/{id}/archive
PATCH /api/Projects/{id}/restore
```

---

## Project Members

```http
GET    /api/projects/{projectId}/members
GET    /api/projects/{projectId}/members/available-users
POST   /api/projects/{projectId}/members
DELETE /api/projects/{projectId}/members/{projectMemberId}
```

---

## Bugs

```http
GET    /api/projects/{projectId}/bugs
POST   /api/projects/{projectId}/bugs

GET    /api/bugs/{bugId}
GET    /api/bugs/my-assigned

PUT    /api/bugs/{bugId}
DELETE /api/bugs/{bugId}

PATCH  /api/bugs/{bugId}/assign
PATCH  /api/bugs/{bugId}/status
PATCH  /api/bugs/{bugId}/priority
```

Additional endpoints are available for:

- comments
- role-specific dashboards

Swagger/OpenAPI is enabled while the backend runs in the Development environment.

---

# 💻 Running the Project Locally

## Prerequisites

Install the following software before starting:

- Git
- .NET 10 SDK
- Microsoft SQL Server / SQL Server Express
- Node.js
- npm
- Visual Studio, VS Code or another editor

Optional:

- SQL Server Management Studio
- EF Core CLI tools

---

# 1️⃣ Clone the Repository

```bash
git clone https://github.com/SiddhantGajbhiye15/BugTrackingSystem.git

cd BugTrackingSystem
```

---

# 2️⃣ Backend Setup

Navigate to the backend project:

```bash
cd backend/BugTrackingSystem
```

Restore NuGet packages:

```bash
dotnet restore
```

---

# 3️⃣ Configure SQL Server

The application requires a SQL Server database.

Example SQL Server Express connection string:

```text
Server=localhost\SQLEXPRESS;
Database=BugTrackingSystemDB;
Trusted_Connection=True;
TrustServerCertificate=True;
```

Change the SQL Server name according to your environment.

---

# 4️⃣ Configure Local Secrets

Do **not** commit production passwords, JWT keys or database credentials to Git.

For local development, .NET User Secrets can be used.

Initialize User Secrets:

```bash
dotnet user-secrets init
```

Configure the database:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=BugTrackingSystemDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

Configure JWT:

```bash
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_LONG_RANDOM_SECRET_KEY"
dotnet user-secrets set "Jwt:Issuer" "BugTrackingSystem"
dotnet user-secrets set "Jwt:Audience" "BugTrackingSystemUsers"
```

Configure the frontend origin:

```bash
dotnet user-secrets set "FrontendUrl" "http://localhost:5173"
```

---

# 5️⃣ Configure Initial Admin

The application supports initial Admin seeding.

Configure an Admin account:

```bash
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "ChangeThisPassword@123"
dotnet user-secrets set "AdminSeed:FirstName" "System"
dotnet user-secrets set "AdminSeed:LastName" "Admin"
```

The Admin is created only when an Admin account does not already exist.

After logging in, the Admin can create Project Managers, Developers and Testers.

---

# 6️⃣ Create / Update the Database

If the EF Core CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Apply the existing migrations:

```bash
dotnet ef database update
```

This creates/updates the `BugTrackingSystemDB` database using the migrations included in the project.

---

# 7️⃣ Run the Backend

Run:

```bash
dotnet run --launch-profile https
```

Development URLs include:

```text
https://localhost:7294
http://localhost:5078
```

Swagger:

```text
https://localhost:7294/swagger
```

---

# 8️⃣ Frontend Setup

Open another terminal and move to the frontend:

```bash
cd frontend
```

Install dependencies:

```bash
npm install
```

---

# 9️⃣ Configure Frontend Environment

Create:

```text
frontend/.env.local
```

Add:

```env
VITE_API_BASE_URL=https://localhost:7294
```

The Axios client reads `VITE_API_BASE_URL` and automatically attaches the JWT token to authenticated requests.

---

# 🔟 Run the Frontend

```bash
npm run dev
```

Open:

```text
http://localhost:5173
```

You can now login using the Admin account created through the initial seed configuration.

---

# 🧪 Development Commands

## Frontend

Start development server:

```bash
npm run dev
```

Build production frontend:

```bash
npm run build
```

Run ESLint:

```bash
npm run lint
```

Preview production build:

```bash
npm run preview
```

---

## Backend

Restore dependencies:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Run:

```bash
dotnet run
```

Apply migrations:

```bash
dotnet ef database update
```

Create a migration:

```bash
dotnet ef migrations add MigrationName
```

---

# 🔧 Configuration Reference

## Backend Configuration

| Configuration | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server connection |
| `Jwt:Key` | JWT signing secret |
| `Jwt:Issuer` | JWT issuer |
| `Jwt:Audience` | JWT audience |
| `FrontendUrl` | Allowed deployed frontend URL |
| `AdminSeed:Email` | Initial Admin email |
| `AdminSeed:Password` | Initial Admin password |
| `AdminSeed:FirstName` | Initial Admin first name |
| `AdminSeed:LastName` | Initial Admin last name |

---

## Frontend Configuration

| Variable | Description |
|---|---|
| `VITE_API_BASE_URL` | Base URL of the ASP.NET Core API |

Example:

```env
VITE_API_BASE_URL=https://localhost:7294
```

---

# 🔒 Security Design

The application includes several security measures.

### Authentication

JWT Bearer authentication is used for API access.

### Authorization

ASP.NET Core role authorization protects sensitive endpoints.

Examples:

```text
Admin
ProjectManager
Developer
Tester
```

### Password Storage

Passwords are hashed before being stored.

Plain-text passwords are not stored in the database.

### Frontend Authentication

Axios automatically attaches:

```http
Authorization: Bearer <JWT>
```

to authenticated API requests.

When the backend returns `401 Unauthorized`, local authentication information is removed and the user is redirected to login.

### Server-Side Business Rules

Important permissions are checked by the backend.

Hiding a button in React is **not** treated as security.

---

# 🚀 Production Deployment

## Frontend — Vercel

The frontend is deployed using Vercel.

Production environment variable:

```env
VITE_API_BASE_URL=https://bugtrackingsiddhant.runasp.net
```

Typical configuration:

```text
Root Directory: frontend
Build Command: npm run build
Output Directory: dist
```

Vercel automatically builds new frontend deployments when changes are pushed to the connected GitHub branch.

---

## Backend

The ASP.NET Core API is deployed separately.

Production configuration should provide:

```text
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
FrontendUrl
AdminSeed__Email
AdminSeed__Password
AdminSeed__FirstName
AdminSeed__LastName
```

Secrets should be configured through the hosting provider and should never be committed to GitHub.

---

# 📈 Current Project Highlights

This project demonstrates:

- Full-stack development
- ASP.NET Core REST API design
- React application development
- SQL Server database design
- Entity Framework Core
- LINQ
- Repository pattern
- Service layer architecture
- DTO-based API contracts
- JWT authentication
- Role-based authorization
- Real-world workflow validation
- Database relationships
- EF Core migrations
- Axios API integration
- Role-based dashboards
- Git/GitHub workflow
- Frontend deployment
- Backend deployment

---

# 🛣️ Future Improvements

Possible future improvements include:

- complete membership-history UI
- support for multiple separate membership periods
- project audit logs
- bug activity timeline
- notification system
- email notifications
- file uploads for bug screenshots
- cloud-based evidence storage
- pagination
- advanced analytics
- automated unit tests
- integration tests
- end-to-end frontend tests
- refresh tokens
- forgot-password flow
- real-time notifications
- project activity reports
- Docker support
- CI/CD pipeline

---

# 🤝 Contributing

Contributions, suggestions and improvements are welcome.

### Development workflow

1. Fork the repository.
2. Create a feature branch.

```bash
git checkout -b feature/your-feature
```

3. Make your changes.
4. Test the backend and frontend.
5. Commit your changes.

```bash
git commit -m "feat: describe your change"
```

6. Push the branch.

```bash
git push origin feature/your-feature
```

7. Open a Pull Request.

Please avoid committing:

- passwords
- JWT signing keys
- production connection strings
- API secrets
- private environment files

---

# 🐛 Reporting Issues

If you find a bug in the project:

1. Open a GitHub Issue.
2. Describe the problem.
3. Add steps to reproduce it.
4. Include expected and actual behaviour.
5. Add screenshots or logs when useful.

---

# 📜 License

This repository does not currently contain an explicit open-source license.

If you intend to allow others to freely use, modify and distribute the project, add a `LICENSE` file.

For a portfolio/open-source project like this, the **MIT License** is a common option.

Once a license is added, update this section accordingly.

---

# 👨‍💻 Author

## Siddhant Gajbhiye

Full-Stack Software Developer

Technologies used in this project:

```text
C#
ASP.NET Core
Entity Framework Core
SQL Server
React
JavaScript
Tailwind CSS
REST APIs
JWT
Git
GitHub
Vercel
```

GitHub:

https://github.com/SiddhantGajbhiye15

---

# ⭐ Support

If you found this project useful or interesting, consider giving the repository a ⭐.

---

<div align="center">

### 🐞 Bug Tracking System

**Built with ASP.NET Core + React + SQL Server**

[Live Demo](https://bug-tracking-system-ten.vercel.app/login) •
[Repository](https://github.com/SiddhantGajbhiye15/BugTrackingSystem)

</div>
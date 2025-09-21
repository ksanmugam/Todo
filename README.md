# TODO List Application

A modern TODO list application built with Angular 20 + Angular Material frontend and .NET 9 Web API backend.

## Features

* ✅ View TODO items
* ➕ Add new TODO items
* 🗑️ Delete TODO items
* 📱 Responsive Material Design UI
* 🔄 Real-time updates
* ✅ Complete with unit tests

## Tech Stack

### Frontend

* Angular 20
* Angular Material 20
* TypeScript
* RxJS
* Jasmine/Karma for testing

### Backend

* .NET 9 Web API
* C#
* In-memory data storage
* xUnit for testing
* Swagger/OpenAPI documentation

## Prerequisites

* Node.js (v18 or higher)
* .NET 9 SDK
* Git

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Todo
```

### 2. Backend Setup

```bash
cd Todo.Server
dotnet restore
dotnet run
```

The API will be available at [http://localhost:5000](http://localhost:5000)

### 3. Frontend Setup

```bash
cd ../todo.client
npm install
ng serve --open
```

The Angular app will be available at [https://localhost:5173](https://localhost:5173)

## Running Tests

### Backend Tests

```bash
cd Todo.Server.Tests
dotnet test
```

### Frontend Tests

```bash
cd todo.client
npm test
```

## API Endpoints

* `GET /api/todos` - Get all TODO items
* `POST /api/todos` - Create a new TODO item
* `DELETE /api/todos/{id}` - Delete a TODO item

## Project Structure

```
├── Todo.Server/              # .NET Web API Backend
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   └── TodoApi.Tests/
├── Todo.Server/              # Backend unit tests
└── TodoApp/                  # Angular Frontend
    ├── src/
    │   ├── app/
    │   │   ├── components/
    │   │   ├── models/
    │   │   ├── services/
    │   │   └── app.component.*
    │   └── environments/
    └── src/app/**/*.spec.ts   # Frontend unit tests
```

## Architecture Decisions

### Backend

* **Repository Pattern**: For data access abstraction
* **Dependency Injection**: For loose coupling and testability
* **CORS Configuration**: To allow frontend communication
* **DTOs**: For clean API contracts

### Frontend

* **Angular Material**: For consistent, accessible UI components
* **Reactive Forms**: For form handling and validation
* **Services**: For API communication and state management
* **Component Architecture**: Following Angular best practices
* **RxJS**: For reactive programming patterns

## Additional Notes

* The backend uses in-memory storage, so data will be reset when the API restarts
* CORS is configured to allow requests from [https://localhost:5173](https://localhost:5173)
* Swagger UI is available at [http://localhost:5000/swagger](http://localhost:5000/swagger) when running the API
* The application follows Material Design principles for optimal UX

## Recommended Running Setup (Best Experience)

### Backend: Visual Studio (Recommended)

1. Open Todo.Server.sln in Visual Studio.
2. Set Todo.Server as the **Startup Project**.
3. Press **F5** to run in debug mode or **Ctrl+F5** to run without debugging.
4. Swagger UI will be available at [http://localhost:5000/swagger](http://localhost:5000/swagger) for quick API testing.
5. Now you may also navigate to [https://localhost:5173/](https://localhost:5173/) to access the Angular frontend.

*Optional CLI approach:*

```bash
cd Todo.Server
dotnet restore
dotnet run
```

### Frontend: VS Code

1. Open todo.client folder in VS Code.
2. Install dependencies:

```bash
npm install
```

3. Run Angular app:

```bash
ng serve
```

* Opens at [https://localhost:5173](https://localhost:5173) by default.
* Connects to backend running on [http://localhost:5000](http://localhost:5000).

### Recommended Workflow

* **Backend in Visual Studio**: Full debugging, breakpoints, code navigation.
* **Frontend in VS Code**: Hot reload with Angular CLI.
* Run both simultaneously for live development.
* Ensure Angular `environment.ts` points to the correct backend URL.

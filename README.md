# 🧾 Minimal POS System (Point of Sale)

A lightweight and scalable Point of Sale (POS) system built with **ASP.NET Core**, following **Clean Architecture** principles. The solution leverages **RabbitMQ**, **Redis**, **Docker**, **MySQL**, **Serilog**, and a **Generic Repository Pattern** for extensibility and maintainability.

---

## 🛠️ Tech Stack

| Technology        | Description                                      |
|-------------------|--------------------------------------------------|
| ASP.NET Core 8     | Backend framework for building APIs              |
| MySQL             | Relational database for data persistence         |
| RabbitMQ          | Messaging broker for async processing (e.g., stock updates) |
| Redis             | Caching service for performance                  |
| Docker            | Containerization for consistent environments     |
| Serilog           | Structured logging to file or external sinks     |
| Clean Architecture| Maintainable, testable, and scalable project     |
| Generic Repository| Reusable data access abstraction using EF Core   |

---

## 📦 Features

- ✅ Manage Products, Categories, and Customers
- ✅ Handle Sales with stock decrement via RabbitMQ
- ✅ Caching product catalog with Redis
- ✅ Logging with Serilog to track operations and errors
- ✅ Scalable architecture with Docker containers
- ✅ Separation of concerns using Clean Architecture
- ✅ Built-in Generic Repository pattern

---

## 📂 Project Structure


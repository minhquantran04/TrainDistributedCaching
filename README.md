# 🧠 Distributed Caching with SQL Server in ASP.NET Core

This project demonstrates how to implement **Distributed Caching** using **SQL Server** in an ASP.NET Core Web API. It caches the `Products` list to reduce database load and improve response time.

---

## 🚀 Features

- ✅ ASP.NET Core Web API (.NET 6/7)
- ✅ Entity Framework Core (Database First)
- ✅ Distributed Caching via SQL Server
- ✅ Auto-refresh cache after Create, Update, Delete
- ✅ Postman-ready (no frontend UI)
- ✅ Clean architecture (separated Models, Controllers, Program.cs)

---

## 💡 How It Works

1. When calling `GET /api/products`, the app checks if cached data (`products_all`) exists.
2. If yes → data is deserialized and returned directly from cache (✅ fast).
3. If no → it fetches from the SQL Server database, stores it in cache, and returns it.
4. Any `POST`, `PUT`, `DELETE` operations will remove the cache to ensure consistency.

---

## 🧱 Technologies Used

- ASP.NET Core Web API
- EF Core (SQL Server, Scaffold-DbContext)
- IDistributedCache
- SQL Server (as cache backend, not just main DB)
- Swagger / Postman for testing

---

## ⚙️ Setup Instructions

1. **Create SQL Server Database:**

```sql
CREATE DATABASE CacheDemoDb;
GO

-- Product table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Price DECIMAL(18, 2)
);

-- Cache table for IDistributedCache
CREATE TABLE [dbo].[CacheTable] (
    [Id] NVARCHAR(449) NOT NULL PRIMARY KEY,
    [Value] VARBINARY(MAX) NOT NULL,
    [ExpiresAtTime] DATETIMEOFFSET NOT NULL,
    [SlidingExpirationInSeconds] BIGINT NULL,
    [AbsoluteExpiration] DATETIMEOFFSET NULL
);

# Restaurant App

Starter repository for building Leakhna's Restaurant Web Ordering and Management System.

## Status

Initial ASP.NET Core MVC application scaffold with menu browsing, cart, customer accounts, tax-aware checkout receipts, customer profiles, and a protected administrator dashboard.

## Project Context

- [Product requirements](docs/PRD.md)
- [ADR 0001: Project direction from proposal](docs/adr/0001-project-direction.md)
- [MongoDB persistence plan](docs/MONGODB.md)

## Run Locally

```powershell
dotnet run
```

## Demo Accounts

| Role | Email | Password |
| --- | --- | --- |
| Customer | customer@leakhnas.local | Customer123! |
| Administrator | admin@leakhnas.local | Admin123! |

## Current Features

- Sign up, sign in, sign out, and customer profile pages
- Role-protected administrator dashboard
- Menu browsing with ingredient and calorie details
- Session-based shopping cart
- Checkout with pickup or delivery
- 13% HST calculation
- Credit card, debit card, PayPal, and e-transfer demo fields
- Receipt confirmation with payment status and masked payment summary
- Registered customer receipt history
- Durable local JSON persistence for development
- MongoDB Atlas persistence provider

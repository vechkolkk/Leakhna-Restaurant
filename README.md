# Leakhna's Restaurant

ASP.NET Core MVC restaurant ordering and management system for Leakhna's Restaurant. The app supports customer menu browsing, cart checkout, receipts, account history, verified reviews, and an administrator dashboard for menu, order, customer, and sales management.

## Project Context

This repository follows the original project proposal as the product baseline:

- [Product requirements](docs/PRD.md)
- [ADR 0001: Project direction from proposal](docs/adr/0001-project-direction.md)
- [MongoDB persistence notes](docs/MONGODB.md)

## Tech Stack

- ASP.NET Core MVC and C#
- Razor views, Bootstrap, CSS, and JavaScript
- Cookie authentication with customer/admin roles
- JSON persistence for local development
- Optional MongoDB Atlas persistence through `MongoDB.Driver`

## Run Locally

```powershell
dotnet restore
dotnet run
```

Then open the local URL shown in the terminal, usually:

```text
https://localhost:5001
```

## Demo Accounts

| Role | Email | Password |
| --- | --- | --- |
| Customer | customer@leakhnas.local | Customer123! |
| Administrator | admin@leakhnas.local | Admin123! |

## Feature List

### Customer Experience

- Browse menu items with prices, descriptions, calories, ingredients, prep estimates, dietary tags, and allergens.
- Search and filter menu items by category, availability, dietary tag, and allergens.
- Add dishes to a cart and update quantities.
- Checkout as a guest or signed-in customer.
- Choose pickup or delivery, ASAP or scheduled fulfillment.
- Capture demo payment methods: credit card, debit card, PayPal, and e-transfer.
- Calculate 13% HST and generate receipt confirmations.
- Register, sign in, update profile details, and change password.
- View receipt history, reorder previous orders, and cancel eligible orders.
- Save favorite dishes.
- Leave reviews only for dishes the customer has purchased.

### Administrator Experience

- Role-protected admin dashboard.
- Customers do not see the admin navigation tab.
- View live dashboard metrics for receipts, active orders, customers, sales, tax, and average order value.
- Manage active order queue and update order status.
- Add, edit, and delete menu items.
- Track restaurant-style availability levels: regular, limited, sold out, and unavailable.
- View compact receipt records with payment, status, fulfillment, and totals.
- Filter sales reports by date, payment method, status, and search.
- Export filtered sales data to CSV.
- Review payment/status breakdowns, menu performance, top dishes, and daily sales.
- Delete inappropriate customer reviews.

## Persistence

The default local provider is JSON:

```json
"Persistence": {
  "Provider": "Json",
  "JsonDataPath": "App_Data/restaurant-data.json"
}
```

`App_Data/restaurant-data.json` is created automatically during local use and is ignored by Git.

MongoDB Atlas can be enabled by setting the provider to `MongoDb` and configuring the connection string outside source control. See [docs/MONGODB.md](docs/MONGODB.md).

## Demo Flow

1. Open the menu and inspect a dish details page.
2. Add one or more dishes to the cart.
3. Checkout as a guest to show receipt generation.
4. Sign in as the demo customer and place another order.
5. Open the customer profile to show order history and reorder/cancel actions.
6. Leave a review for a purchased dish.
7. Sign in as the administrator.
8. Show the dashboard metrics, active order queue, menu management, reports, CSV export, and review moderation.

## Notes

- This app uses demo payment capture only; it does not charge real cards or connect to a live payment provider.
- Seed data is created when the configured data store is empty.
- Local generated data can be reset by stopping the app and deleting `App_Data/restaurant-data.json`.

# MongoDB Persistence Plan

The app now has a persistence boundary and durable local JSON storage so users, orders, receipts, and menu data survive app restarts during development.

MongoDB Atlas is the target database from the proposal. The MongoDB-backed store is implemented and can be enabled after configuring an Atlas connection string.

```powershell
dotnet add package MongoDB.Driver
```

The package is required for the MongoDB provider.

## Current Local Persistence

The default provider is configured in `appsettings.json`:

```json
"Persistence": {
  "Provider": "Json",
  "JsonDataPath": "App_Data/restaurant-data.json"
}
```

`App_Data/restaurant-data.json` is generated locally and ignored by Git.

## MongoDB Atlas Configuration

Use:

```json
"MongoDb": {
  "ConnectionString": "<your Atlas connection string>",
  "DatabaseName": "LeakhnasRestaurant",
  "UsersCollection": "users",
  "MenuItemsCollection": "menuItems",
  "OrdersCollection": "orders"
}
```

Do not commit real connection strings. Store them with user secrets or environment variables.

Then set:

```json
"Persistence": {
  "Provider": "MongoDb",
  "JsonDataPath": "App_Data/restaurant-data.json"
}
```

## Implemented MongoDB Behavior

- Seeds admin/customer/menu data when collections are empty.
- Uses MongoDB for users, menu items, orders, and receipts.
- Adds indexes for user email, order id, customer id, and menu category.
- Keeps JSON persistence available for local development without Atlas.

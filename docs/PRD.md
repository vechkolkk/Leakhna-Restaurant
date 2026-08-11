# Product Requirements Document: Leakhna's Restaurant

## Product Summary

Leakhna's Restaurant Web Ordering and Management System is a web-based ordering and administrative platform for a small restaurant. It is intended to improve customer convenience and restaurant operations by supporting online menu browsing, ordering, payment handling, receipt access, menu management, and sales reporting.

This document is adapted from the original project proposal by Vechkol Khourn.

## Problem Statement

Many small restaurants still rely on paper-based orders, manual receipts, and limited payment options. This makes it difficult to manage sales records and provide a convenient customer experience.

Customers also often lack access to detailed menu information, including ingredients and nutritional values. Restaurant owners need better tools to track orders, receipts, menu items, and sales records efficiently.

## Goals

- Allow customers to browse menu items and view detailed food information.
- Support registered customer accounts and guest checkout.
- Let customers place online orders and complete payment.
- Generate receipts for completed orders.
- Provide registered users with order and receipt history.
- Give restaurant administrators tools to manage menu items, orders, receipts, and reports.
- Support sales and receipt reporting for restaurant operations.

## Target Users

- Registered restaurant customers
- Guest customers
- Restaurant owner or administrator

## Primary Use Cases

### Registered Customer

- Create and manage an account.
- Browse the restaurant menu.
- View ingredients and estimated calories for each item.
- View prep estimates, dietary tags, and allergen warnings.
- Add items to a cart.
- Place online orders.
- Pay using multiple payment methods.
- View order and receipt history.
- Reorder previous purchases.
- Save favorite dishes and review purchased dishes.

### Guest Customer

- Browse the restaurant menu.
- View ingredients and estimated calories.
- Place an order without creating an account.
- Receive a one-time receipt after payment.

### Administrator

- Manage menu items and categories.
- Add, edit, or remove food items.
- Update ingredients and calorie information.
- Track dish availability using restaurant-friendly levels such as regular, limited, sold out, and unavailable.
- View all orders and receipts.
- Categorize receipts by payment type, date, and order status.
- Generate sales reports.
- Moderate customer reviews.

## Core Features

- User authentication and authorization
- Guest checkout
- Shopping cart
- Order management
- Receipt generation
- Receipt history for registered users
- Menu display with ingredients
- Calorie information display
- Prep time estimates
- Dietary and allergen information
- Favorites
- Verified-purchase reviews
- Payment processing
- Administrative dashboard
- Menu availability management
- Active order queue
- Sales and receipt reporting

## Payment Requirements

The system should support:

- Credit card
- Debit card
- PayPal
- E-transfer record and verification support

## Technical Direction

- Front end: ASP.NET Core MVC, HTML5, CSS3, Bootstrap, JavaScript
- Back end: ASP.NET Core, C#
- Authentication: ASP.NET Core cookie authentication with customer and administrator roles
- Database: MongoDB Atlas using MongoDB collections
- Data access: MongoDB Driver for .NET

## Delivery Roadmap

| Weeks | Focus |
| --- | --- |
| 1-2 | Requirements gathering, project planning, system design, and database setup |
| 3-4 | ASP.NET project setup, MongoDB integration, authentication, and menu management |
| 5-6 | Shopping cart, guest checkout, ordering system, and payment integration |
| 7-8 | Receipt generation, order history, and administrator dashboard development |
| 9-10 | Testing, debugging, reporting features, deployment, documentation, and presentation preparation |

## Product Decisions

- Payments are demo captures only for this version; no real payment provider is connected.
- PayPal and e-transfer are recorded as payment methods, not fully automated transactions.
- Orders support both pickup and delivery.
- The first version uses two roles: customer and administrator.
- Sales reports include date, payment method, status, search filtering, CSV export, payment/status breakdowns, top dishes, and daily sales.
- Calories and prep times are administrator-provided estimates.

# ADR 0001: Project Direction From Proposal

## Status

Accepted

## Context

The original project proposal defines Leakhna's Restaurant Web Ordering and Management System as a web application for customer ordering and restaurant administration. The system is intended to replace paper-based ordering and manual receipt workflows with a modern online ordering experience.

The proposed implementation stack is ASP.NET Core MVC with C#, ASP.NET Identity, MongoDB Atlas, and the MongoDB Driver for .NET.

## Decision

Use the proposal as the initial product and architecture baseline for this repository.

The first version of the application should be built around:

- ASP.NET Core MVC for the web application structure.
- C# and ASP.NET Core for server-side behavior.
- ASP.NET Identity for registered customer and administrator authentication.
- MongoDB Atlas for cloud-hosted persistence.
- MongoDB collections for menu items, customers, carts, orders, receipts, and reporting data.
- Bootstrap, CSS, and JavaScript for the customer and administrator interfaces.

The product scope should prioritize customer ordering, guest checkout, receipt generation, registered-user history, menu item details, and administrator menu/order management before broader reporting or operational features.

## Consequences

- The repository should be organized around an ASP.NET Core MVC application rather than a static site or single-page app first.
- Domain models should reflect restaurant concepts such as menu items, categories, carts, orders, payments, receipts, and users.
- Authentication and authorization need to be designed early because the system has customer, guest, and administrator workflows.
- Payment support should be abstracted enough to handle multiple methods, including card payments, PayPal, and e-transfer verification.
- MongoDB document design should be considered before building reporting features, since receipts and order history are central requirements.

## Deferred Decisions

- Specific payment provider and payment verification workflow.
- Deployment platform.
- Exact MongoDB collection schemas and indexes.
- Whether the first release supports pickup, delivery, or both.
- Administrator dashboard report definitions.

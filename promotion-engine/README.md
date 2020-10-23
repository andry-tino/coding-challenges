# [Challenge](Challenge.md), .NET Core library + ASP.NET Core Api
October 2020.

This includes:

- A .NET Core library (written in C#) as required by the assignment.
- An ASP.NET Core web Api used as an example of usage of the library.

## Requirements
To be able to build the library and build & run the application, you need:

- [Microsoft .NET Core](https://dotnet.microsoft.com/download) 3.1.
- [Microsoft Visual Studio](https://visualstudio.microsoft.com/) (whatever flavor, latest version).
- An operating system targeted by the .NET Core: Windows, MacOS or Linux.

To interact with the web API, you will need:

- An HTTP client (fx. [Postman](https://www.postman.com/), [Fiddler](https://www.telerik.com/download/fiddler), etc.).

## Get
To get the application, you can:

- [Clone the Challenges repository](https://github.com/andry-tino/coding-challenges.git).

The code will be inside the `promotion-engine` directory.

## Build
Two solutions have been created:

- `PromoEngLibrary.sln`: library only.
- `PromoEng.sln`: library + example app (web API).

You have these options:

- If you want to build the library and run its tests, open `PromoEngLibrary.sln` in Visual Studio, build and run tests.
- If you want to test the library using the example web API, open `PromoEng.sln` in Visual Studio, build and run tests. To run the web app, choose the `PromoEng.CoreWebApi` as run configuration (after selecting the `PromoEng.CoreWebApi` project) and run it.

By default, the server will run on port `5000` for HTTP and port `5001` for HTTPS (recommended).

The web app will start with configuration `appsettings.json`. In there, you can find the default sequence of rules being set and the SKU collection. You can change it and restart the app to use a different sequence of rules and a different set of SKU.

---

## The library
The library consists of the following key components:

- `ICart`: An interface describing the general behavior of a cart:
    - Storing SKUs.
    - Merging with another cart.
    - Providing info on the collection.
    - Adding SKUs to the collection.
- `Sku`: A type representing a single SKU for exclusive identification purposes.
- `IPromotionRule`: Interface describing the general behavior of a promotion rule. Promotion rules are thought as atomic and having no side effects on carts. They take a cart as input and, without changing it, return a different cart. Promotion rules are mutually exclusive: one SKU in cart can be processed by one rule only.
- `IPromotionPipeline`: An interface describing the general behavior of a `promotion pipeline`, that is: a sequential application of promotion rules in a specific order. Pipelines require rules to be added. When applied to a cart, a new cart is returned as a result of the application of all the rules, in order.

The library provides concrete types implementing the abtractions described above. See comments in each of those components for more info.

**Note:** The challenge provides 3 test cases which have been coded in a scenario suite: `/promotion-engine/PromoEng/PromoEng.Engine.Scenarios/DifferentRulesScenarios.cs`.

Design goals:

1. **Immutability of carts with respect to rules:** Considering the library has application in contexts involving money and the purchase/selling of goods, design has been optimized for **transactional scenarios**. Rules treat carts as immutable objects. This adds a cost in terms of GC pressure.
2. **Complexity:** Rules execute with linear complexity with respect to the number of items in the cart.

---

## The web API
The web server exposes the following endpoints:

- `/api`: Information about the API.
- `/api/cart`: Interact with the application to create and manage carts.

## Cart handling API
The following functionalities are available:

- `GET /api/cart`: Get all carts (only info).
- `GET /api/cart/<id>`: Get a specific cart (info + content).
- `POST /api/cart`: Create a new cart.
- `POST /api/cart/<id>`: Checkout cart.
- `PUT /api/cart/<id>`: Add SKU to cart.
- `DELETE /api/cart/<id>`: Delete (not checked out) cart.

Swagger is available at `/api` to get more info.
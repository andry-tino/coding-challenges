# [Challenge](Challenge.md), .NET Core library + ASP.NET Core Api
October 2020.

## Requirements
To be able to build the li8brary and build and run the application, you need:

- [Microsoft .NET Core](https://dotnet.microsoft.com/download) 3.1.
- [Microsoft Visual Studio](https://visualstudio.microsoft.com/) (whatever flavor, latest version).
- An operating system targeted by the .NET Core: WIndows, MacOS or Linux.

To interact with the web API, you will need either:

- An HTTP client (fx. [Postman](https://www.postman.com/), [Fiddler](https://www.telerik.com/download/fiddler), etc.).

## Get
To get the application, you can:

- [Clone the Challenges repository](https://github.com/andry-tino/coding-challenges.git).

## Build
Two solutions have been set up:

- `PromoEngLibrary.sln`: library only.
- `PromoEng.sln`: library + example app (web API).

You have these options:
- If you want to build the library and run its tests, open `PromoEngLibrary.sln` in Visual Studio, build and run tests.
- If you want to test the library using the example web API, open `PromoEng.sln` in Visual Studio, build and run tests. To run the web app, choose the `PromoEng.CoreWebApi` as run configuration (after selecting the `PromoEng.CoreWebApi` project) and run it.

By default, the server will run on port `5000` for HTTP (only in `Development` environment) and port `5001` for HTTPS (recommended).

The web app will start with configuration `appsettings.json`. In there, you can find the default sequence of rules being set and the SKU collection. You can change it and restart the app to use a different sequence of rules and a different set of SKU.

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
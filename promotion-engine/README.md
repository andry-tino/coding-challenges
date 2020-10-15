# [Challenge](Challenge.md), ASP.NET Core App
October 2020.

## Requirements
To be able to build and run the application, you need:

- [Microsoft .NET Core](https://dotnet.microsoft.com/download) 3.1.
- [Microsoft Visual Studio](https://visualstudio.microsoft.com/) (whatever flavor, latest version).
- An operating system targeted by the .NET Core: WIndows, MacOS or Linux.

To interact with the web API, you will need either:

- An HTTP client (fx. [Postman](https://www.postman.com/), [Fiddler](https://www.telerik.com/download/fiddler), etc.).
- The [TODO ReactClient](TODO.ReactClient/README.md).

## Get
To get the application, you can either:

- [Clone the Challenges repository](https://github.com/andry-tino/coding-challenges.git).
- Use the binaries already available as [releases](https://github.com/andry-tino/coding-challenges/releases). It is recommended to pick up the latest version among those available.

## Build
If you already got the binaries, you can skip this.
You can either:

- Build the server using Visual Studio.
- Build the server using the commandline.

### Building the server in Visual Studio
To build the server using Visual Studio:

1. Open the solution `TODO.sln` in Visual Studio.
2. Select configuration `Release`.
3. Build the solution by selecting: `Build`, `Build Solution` in Visual Studio.

### Building the server using `dotnet`
To build the server using the commandline:

1. Open a new shell window.
2. Navigate to `TODO`.
3. Run command: `dotnet publish -c Release`.

## Running the server
To run the server you can either:

- Use Visual Studio to run the application (after building it, see previous steps).
- Use the binaries already available.

By default, the server will run on port `5000` for HTTP (only in `Development` environment) and port `5001` for HTTPS (recommended).

### Running in Visual Studio
If you chose to build the solution, after doing so, do the following:

1. Select the `TODO` configuration.
    - Use the `TODO` configuration to get the `Development` environment and be able to use HTTP.
2. Select the `Run` button.

### Running using `dotnet`
If you want to use the commandline and you built the code using `dotnet`:

1. Navigate to `TODO`.
2. Run command: `dotnet run --configuration Release --launch-profile TODO`.
    - Use option `--launch-profile TODO` to get the `Development` environment and be able to use HTTP.

### Running the binaries
If you got the binaries, no build was required. Do the following:

1. In the folder you got, open a new shell.
2. Set environment variable `ASPNETCORE_ENVIRONMENT` to `"Production"`.
3. Run: `dotnet .\TODO.dll`.

---

## Using TODO
WIS is a server exposing a web API to sort integer sequences.

TODO
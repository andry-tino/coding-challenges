# [Marsk 2020 Challenge](Challenge.md), ASP.NET Core App
October 2020.

## Requirements
To be able to build and run the application, you need:

- [Microsoft .NET Core](https://dotnet.microsoft.com/download) 3.1.
- [Microsoft Visual Studio](https://visualstudio.microsoft.com/) (whatever flavor, latest version).
- An operating system targeted by the .NET Core: WIndows, MacOS or Linux.

To interact with the web API, you will need either:

- An HTTP client (fx. [Postman](https://www.postman.com/), [Fiddler](https://www.telerik.com/download/fiddler), etc.).
- The [WebIntSorter ReactClient](WebIntSorter/WebIntSorter.ReactClient/README.md).

## Building the server in Visual Studio
To build the server:

1. Open the solution `WebIntSorter.sln` in Visual Studio.
2. Select configuration `Release`.
3. Build the solution by selecting: `Build`, `Build Solution` in Visual Studio.

## Building the server using `dotnet`
To build the server:

1. Open a new shell window.
2. Navigate to `maersk2020.dotnet/WebIntSorter/WebIntSorter`.
3. Run command: `dotnet publish -c Release`.

## Running the server
To run the server you can either:

- Use Visual Studio to build the solution and then run the application.
- Use the binaries already available as [releases](https://github.com/andry-tino/coding-challenges/releases). It is recommended to pick up the latest version among those available.

By default, the server will run on port `5000` for HTTP and port `5001` for HTTPS (recommended).

### Running in Visual Studio
If you choose to build on your own the solution, after doing so, do the following:

1. Select the `WebIntSorter` configuration.
2. Select the `Run` button.

### Running using `dotnet`
If you want to use the commandline:

1. Navigate to `maersk2020.dotnet/WebIntSorter/WebIntSorter`.
2. Run command: `dotnet .\bin\Release\netcoreapp3.1\Maersk2020DotNet.WebIntSorter.dll`.

## Using WebIntSorter
WIS is a server exposing a web API to sort integer sequences.

- Two routes are available: `/api`, `/api/sorting`.
- Allowed verbs  (HTTP methods) for `/api/sorting` are:
    - `POST`: Enqueue a sorting job.
	- `GET`: Get info about all the enqueued jobs or one specific job.
- `Content-Type`: `application/json`.

Submit a `GET` request against `/api` to get a list of all supported routes in the API.

### Enqueueing a sorting job
To enqueue a sorting job, submit a `POST` request to `/api/sorting` by specifying the sequence to sort onside an array of integers:

```
POST /api/sorting HTTP/1.1
Host: localhost:5001
Content-Type: application/json

[1,5,3,7,3,45,4,67,53,0]
```

The response will be sent immediately:

```
{
    "id": 3
}
```

It will contain the ID of the job just enqueued.

### Retrieving jobs info
To retrieve the list of enqueued jobs and their info, submit a `GET` request to `/api/sorting`:

```
GET /api/sorting/ HTTP/1.1
Host: localhost:5001
Content-Type: application/json
```

The response:

```
[
	{
		"id": 6e0b0385-2bbe-447b-a7e3-aa2bd5cec1af,
		"timestamp": "2020-10-04T17:07:39.1873311+02:00",
		"duration": 150,
		"status": 1,
		"values": [...],
		"originalValues": [...]
	},
	...
]
```

It will include an array of object, each representing a job:

```
{
	"id": string,
	"timestamp": string,
	"duration": number,
	"status": number,
	"values": number[],
	"originalValues": number[]
}
```

- `id`: The unique identifier of the job.
- `timestamp`: The date and time when the job was enqueued.
- `duration`: The time, in milliseconds, elapsed from the moment the job was enqueued, until the sorting has been completed.
- `status`: The status of the job.
    - `0`: Pending, the job is still running.
	- `1`: Completed successfully.
	- `2`: Failed.
- `values`: The sorted sequence if the status is `1`, `null` otherwise.
- `originalValues`: The original sequence provided when the job was enqueued.

If no jobs have been enqueued, the response body will be an empty aray `[]`.

### Retrieving one specific job
To retrieve one specific job, submit a `GET` request to `/api/sorting/{job-id}`, where
`{job-id}` is the ID of the job enqueued previously:

```
GET /api/sorting/6e0b0385-2bbe-447b-a7e3-aa2bd5cec1af HTTP/1.1
Host: localhost:5001
Content-Type: application/json
```

The response:

```
{
	"id": 6e0b0385-2bbe-447b-a7e3-aa2bd5cec1af,
	"timestamp": "2020-10-04T17:07:39.1873311+02:00",
	"duration": 150,
	"status": 1,
	"values": [...],
	"originalValues": [...]
}
```

It will include only one object representing the requested job.

If the requested job (the job ID) does not exist, the response body will be empty and the response status will be `204 No Content`.

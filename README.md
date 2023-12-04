# packetmail-api

The start of a REST API around BPQ32's telnet BBS interface.

Nothing more than a proof of concept at this point.

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Y8Y8KFHA0)

## Installation

### Prerequisites
  - .NET 8 SDK
  - LinBPQ with telnet port

### Building from source

```
git clone https://github.com/M0LTE/packetmail-api.git
cd packetmail-api
dotnet build
```

### Configuration
tbc

(you need to specify variables NODE_TELNET_CTEXT, BPQ_HOST, BPQ_TELNET_PORT)

### Installation

One day, this might be an `apt install packetmail-api`. For now, you're on your own. Start with a `dotnet run`.

## Requests

### Login

To start a session, which can be long-lived, make a request to the /login endpoint with your BPQ telnet interface credentials:

```
curl -X POST "http://localhost:5009/login?bpqTelnetUsername=username&password=password"
```

The response code, if successful, will be 200, and the response body will be a session token. You'll need this for your next request.

### Mail summary

To get a JSON blob containing a summary of your mail, make a request like this:

```
curl http://localhost:5009/mail -H "sessionToken: aaa"
```

where `aaa` is the session token you received when you POSTed to `/login`.

#### Example response

```
[
  {
    "id": 4047,
    "date": "16-Nov",
    "type": "PN",
    "length": 197,
    "destinationCall": "M0LTE",
    "destinationBbs": "@GB7RDG",
    "sourceCall": "GM5AUG",
    "subject": "Testing"
  },
  {
    "id": 4043,
    "date": "02-Nov",
    "type": "PY",
    "length": 184,
    "destinationCall": "M0LTE",
    "destinationBbs": "@GB7RDG",
    "sourceCall": "GM5AUG",
    "subject": "Test of HF forwarding"
  },
  {
    "id": 4041,
    "date": "01-Nov",
    "type": "PY",
    "length": 305,
    "destinationCall": "M0LTE",
    "destinationBbs": "@GB7RDG",
    "sourceCall": "GM5AUG",
    "subject": "Hello Tom"
  }
]
```

### Mail item

There is not yet an endpoint implemented to allow one to fetch an item of mail.

## Roadmap
  - Implement enough endpoints to allow someone to build a reasonable packet mail client, starting with reading, and progressing to writing
  - Back ends for BBS implementations other than BPQ telnet?

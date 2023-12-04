# packetmail-api

The start of a REST API around BPQ32's telnet BBS interface.

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

One day, this might be an `apt install packetmail-api`. For now, you're on your own.

## Requests

### Login

To start a session, which can be long-lived, make a request to the /login endpoint with your BPQ telnet interface credentials:

```
curl -X POST "http://localhost:5009/login?bpqTelnetUsername=username&password=password"
```

The response code, if successful, will be 200, and the response body will be a session token. You'll need this for your next request.

### Mail

To get a JSON blob containing a summary of your mail, make a request like this:

```
curl http://localhost:5009/mail -H 'sessionToken: aaa'
```

where `aaa` is the session token you received when you POSTed to `/login`.

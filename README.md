# packetmail-api

The start of a REST API around BPQ32's telnet BBS interface.

Nothing more than a proof of concept at this point. Just a wrapper around login via telnet, entering the BBS application, issuing an `lm` command, parsing the results into JSON, and returning them to the client.

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
curl http://localhost:5009/mail -H "sessionToken: {aaa}"
```

where `{aaa}` is the session token you received when you POSTed to `/login`.

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

To get a JSON blob containing a particular item of mail, make a request like this:

```
curl http://localhost:5009/mail/{id} -H "sessionToken: {aaa}"
```

where `{id}` is the id of the mail item and `{aaa}` is the session token you received when you POSTed to `/login`.

#### Example response

```
{
  "from": "GM5AUG",
  "to": "M0LTE",
  "typeStatus": "PY",
  "dateTime": "16-Nov 12:26Z",
  "bid": "623_GB7AUG",
  "title": "Testing",
  "routing": [
    "R:231116/1228Z 1892@GB7IOW.GB7IOW.#48.GBR.EURO LinBPQ6.0.24",
    "R:231116/1226Z 623@GB7AUG.#78.GBR.EURO LinBPQ6.0.24"
  ],
  "body": "Hey Tom\nJust checking the node is back up and running.\nHave a good day!\n73 M"
}
```

### Send mail

To send a message, make a request like this:

```
curl -X POST http://localhost:5009/mail \
  -H "sessionToken: {aaa}" \
  -H "Content-Type: application/json" \
  -d '{
  "to": "M0LTE",
  "title": "Subject here",
  "body": "This is line 1\nAnd this is line 2"
}'
```

where `{aaa}` is the session token you received when you POSTed to `/login`, and the body is a JSON blob representing the message to send.

#### Example response

```
{
  "id": 4068,
  "bid": "4068_GB7RDG",
  "size": 8
}
```

## Roadmap
  - Implement enough endpoints to allow someone to build a reasonable packet mail client, starting with reading, and progressing to writing
  - Back ends for BBS implementations other than BPQ telnet?

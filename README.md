# SimpleTransactionalOutbox
C# implementation of the [Transactional outbox](https://microservices.io/patterns/data/transactional-outbox.html) pattern.

![Outbox](Outbox.png) 

* Request to the WebApi changes processing data and creates outbox message in one transaction
* Outbox service reads actual messages and process one by one 
    * Publicates message
    * Removes message from outbox table  

Even if API will be scaled, messages will go to the stream in the correct order.

Incoming http POST message
```
{
    "id":"1",
    "value":"42"
}
```

Kafka stream message
```
{
   "MessageId":"e433fa40-9a18-4bb3-b820-6f027e7f585d",
   "OccurredOn":"2021-04-02T18:51:32.070031",
   "MessageType":0,
   "Body":"{\"Id\":1,\"Value\":42}"
}
```

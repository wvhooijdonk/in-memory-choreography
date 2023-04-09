# In memory choreography

## Goals

* an event can be any class
* classes can publish events by using a publisher
* a listener can be created to receive a specific event
* implementation can be replaced by an AMQP implementation

## Limitations

* all events are queued in a single queue
* limited to the 'fanout exchange' behaviour
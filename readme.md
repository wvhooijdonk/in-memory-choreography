# In memory choreography

## Goal

Provide a way for classes to publish and listen to events within a project so that coupling is minimised

## Constraints

* an event can be any class
* classes can publish events by using a publisher
* classes can listen to events by using a listener
* implementation can be replaced by an AMQP implementation

## Current implementation limitations

* all events are queued in a single queue
* limited to 'fanout exchange' behaviour

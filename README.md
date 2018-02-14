# A Game Of Drones

This repository contains files and solutions for an Azure Functions workshop. Feel free to try things out, comment, submit PRs, we are happy to improve this project and help many people out.

## The Story

It all begins with a story:

> Fabrikam, Inc. is starting a drone delivery service. The company manages a fleet of drone aircraft. Businesses register with the service, and users can request a drone to pick up goods for delivery. When a customer schedules a pickup, a backend system assigns a drone and notifies the user with an estimated delivery time. While the delivery is in progress, the customer can track the location of the drone, with a continuously updated ETA.

(from [the Microsoft docs](https://docs.microsoft.com/en-us/azure/architecture/microservices/))

This sentence alone sparks many details in the Software Engineer's head: what database could I use? Where does the drone data come from? I should use queues to model the workflow!

### Constraints

Here are some constraints that make things easier and harder:

- There are only a limited number of drones available
- Deliveries are always from point A to point B. Remember to also think about how the drone gets from the HQ to point A and back home from point B!
- The status log needs to be available at real time (+/- a few seconds ;) )
- The drones are in perfect conditions and don't break down or have other malfunctions
- A delivery can always have a fixed payload - like a simple string :)  
- A drone can only do one delivery at a time, it's only available after it has returned


However, let's handle one thing at a time and start simple. In order to make the scenario easier to approach we have broken it down into ...

## The Challenges

For implementing this scenario, it's important to focus on core parts first - which is exactly what these levels are designed to do. 

### Level 1

Approaching the problem as a customer, we need two things: an endpoint to request (order) a delivery and one to check on the delivery. Thus - two endpoints are required: `/request` and `/status`. Additionally it comes in handy to have an endpoint where the drone actually can send data while flying, so a third endpoint should be created (and it can have any name you like). 

Don't worry too much about where to store the data, or how to connect the functions and create the workflow if this is your first time going serverless. Level 2 will let you experiment on that, **the goal** now is to get familiar with functions, and that calls to each of the endpoints returns _some_ data. 

#### Order/Request endpoint

There are two minimum parameters: `from` and `to`. These should be implemented as GET parameters - in order to keep things simple. Feel free to get more complex later on!

On calling this endpoint, it would be helpful to have a reference number to check the status with. Please return something like an `order id` that can be used as a lookup reference for the `/status` endpoint.

#### Status endpoint

As an event log or status log (think: DHL tracking), it's imperiative to record date and time as well as what happened. For example this could be a line generated from requesting the drone. JSON makes sure that this is also machine readable.

```json
[{"2/14/2018 2:36:57 PM +00:00":{"type":"requested"}}]
```

#### Drone data endpoint

Since we are experiencing a shortage of real drones, we provide a simulated drone. However this virtual drone needs to provide you with "in flight" data - for simplicity a number that's decremented on a fixed interval - which is done by calling one of your web endpoints. 

On each start, a drone must register its starting with the simulator by issuing a GET request to `https://dronebiz.azurewebsites.net/api/RegisterFlight` providing the parameters `id` (an id for your reference, will be sent with each tick), a `url` (where the drone data endpoint lives), and the projected `flightTime`. The flight time will be decremented and included in the callback. Consequently, the Drone data endpoint has two parameters: `id` (the previously provided id), and `remainingFlightTime`. 

### Level 2

After creating the fundamentals and having a working skeleton of the application, the most vital part of the system needs to be taken care of: connection. Create the workflow using available storage integrations or connectors, feel free to explore the capabilities and limits of each one. 

Each of the function does a specific thing, but how can you integrate them? There are countless ways, think about the introduction and the available integrations. Maybe [Durable Functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable-functions-overview)? Or [Azure queue storage](https://docs.microsoft.com/en-Us/azure/storage/queues/storage-dotnet-how-to-use-queues)? How about [Logic Apps](https://azure.microsoft.com/en-us/services/logic-apps/) to connect them?

Try out different solutions, **the goal** is to get everything working together in an orderly fashion: after calling the `/request` endpoint with the right parameters, a log should be generated in `/status` where each step is recorded live. It starts with the request and ends with the drone returning home after dropping off the payload!

### Level 3

At this point there should be a working application, congratulations! Whatever path you chose, now it's time to explore the limits. How many requests can you issue safely? Are errors well handled? How often do drones try to re-assign a delivery to a drone if none was available? Extend the application to take in a pre-defined payload or scheduled delivery times in the future.  

Take a look into your [hosts.json](https://docs.microsoft.com/en-us/azure/azure-functions/functions-host-json) to fine-tune concurrency and other parameters. 

**The goal** here is to see what is suitable for Functions and what is not, how reliable your application system is and how well everything works together. Explore the capabilities of different languages too if you want!


## Done!

If all went well, Level 1 shouldn't have been too much of a challenge, Level 2 took the longest and Level 3 was only for the really interested. We hope that you learned something within the (typical) 4h workshop and you can now go out and use Functions for whatever real world project. Feel free to ping us if you want us to come by and help!
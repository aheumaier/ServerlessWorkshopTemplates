#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
#r "System.Web"
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System;


public static async Task Run(string pickupItem,  
                             IAsyncCollector<StatusEntry> statusTable,
                             CloudQueue readyQueue,
                             TraceWriter log)
{
    string payload = "Hello World!";
    dynamic ready = JObject.Parse(pickupItem);

    log.Info($"{pickupItem}");

    string assignment = ready.Id;
    string drone = ready.Drone;
    int next = ready.Next;
    string nextLocation = ready.Itiniary[next];

    var _event = new StatusEntry() {
        PartitionKey = "1",
        RowKey = Guid.NewGuid().ToString(),
        Drone = drone,
        Assignment = assignment
    };

    try {
      
        _event.Event = JsonConvert.SerializeObject(new PickUpEvent() {
                Payload = payload,
                Status = "Success"
        });

        readyQueue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(new {
            Id = ready.Id,
            Drone = ready.Drone,
            Next = ready.Next,
            Payload = payload,
            Itiniary = ready.Itiniary
        })));
    }
    catch(Exception ex) {
        log.Error($"Something went wrong: {ex}");

        _event.Event = JsonConvert.SerializeObject(new PickUpEvent() {
                Payload = payload,
                Status = $"Failed: {ex}"
            });
    }
    finally {
        await statusTable.AddAsync(_event);
    }        
}

public class StatusEntry : TableEntity {
    public string Assignment { get; set; }
    public string Drone { get; set;}
    public string Event { get; set; }
}

public class PickUpEvent { 
    public string Payload { get; set; }
    public string Type { get {return "pickup";}}
    public string Status { get; set; }
}
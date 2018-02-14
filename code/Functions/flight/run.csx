#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<NextAction> nextTable, IAsyncCollector<StatusEntry> statusTable, 
                        CloudQueue pickupQueue, CloudQueue deliveryQueue, CloudQueue returnQueue, TraceWriter log)
{
    // parse query
    var parameters = req.GetQueryNameValuePairs();
    string id = parameters 
        .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
        .Value;
    int remainingFlightTime = Int32.Parse(parameters
        .FirstOrDefault(q => string.Compare(q.Key, "remainingFlightTime", true) == 0)
        .Value);

    var _event = new FlightEvent() {
        Remaining = remainingFlightTime,
        Status = "Success"
    };

    NextAction action = null;
    foreach(var next in nextTable.Where(p => p.RowKey == id).ToList()) {
        action = next;
        break;
    }

    if(remainingFlightTime <= 0) {
        var queueItem = new  {
            Id = action.RowKey,
            Drone = action.Drone,
            Next = action.Next, 
            Itiniary = action.Itiniary.Split(',')
        };
        var nextQueue = action.Next < queueItem.Itiniary.Length? queueItem.Itiniary[action.Next]: "";
        var msg = new CloudQueueMessage(JsonConvert.SerializeObject(queueItem));
        switch(nextQueue) {
            case "Pickup":
                pickupQueue.AddMessage(msg, null, null, null, null);
                break;
            case "DropOff":
                deliveryQueue.AddMessage(msg, null, null, null, null);
                break;
            default: 
                returnQueue.AddMessage(msg, null, null, null, null);
                break;
        };
    }
    
    await statusTable.AddAsync(new StatusEntry() {        
        PartitionKey = "1",
        RowKey = Guid.NewGuid().ToString(),
        Drone = action.Drone,
        Assignment = id,
        Event = JsonConvert.SerializeObject(_event)
    });
    return req.CreateResponse(HttpStatusCode.OK,"");
}


public class StatusEntry : TableEntity {
    public string Assignment { get; set; }
    public string Drone { get; set;}
    public string Event { get; set; }
}

public class FlightEvent {
    public int Remaining { get; set; }
    public string Type { get { return "flying";}}
    public string Status { get; set; }
}

public class NextAction: TableEntity {
    public string Drone { get; set; }
    public int Next { get; set; }
    public string Itiniary { get; set; }
}

#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
#r "System.Web"
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


public static async Task Run(string returnMessage,  
                             IQueryable<Drone> currentDrones, 
                             IAsyncCollector<StatusEntry> statusTable,
                             IAsyncCollector<Drone> updatedDrones,
                             TraceWriter log) 
{
    dynamic msg = JObject.Parse(returnMessage);

    log.Info($"{returnMessage}");

    string assignment = msg.Id;
    string drone = msg.Drone;
    int next = msg.Next;
    string nextLocation = msg.Itiniary[next];

    var _event = new StatusEntry() {
        PartitionKey = "1",
        RowKey = Guid.NewGuid().ToString(),
        Drone = drone,
        Assignment = assignment
    };

    Drone assignedDrone = null;
    foreach(var d in currentDrones.Where(p => p.RowKey == drone).ToList()) {
        assignedDrone = d;
        break;
    }

    assignedDrone.Status = "idle";

    try {  
        await updatedDrones.AddAsync(assignedDrone);    
        _event.Event = JsonConvert.SerializeObject(new ReturnEvent() {
                Status = "Success"
        });
    }
    catch(Exception ex) {
        log.Error($"Something went wrong: {ex}");

        _event.Event = JsonConvert.SerializeObject(new ReturnEvent() {
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

public class ReturnEvent { 
    public string Type { get {return "dropoff";}}
    public string Status { get; set; }
}


public class Drone : TableEntity {
    public string Name { get; set; }
    public string Status {get; set;}
}
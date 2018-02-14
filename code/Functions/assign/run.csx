#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
public static async Task Run(string droneRequest,  
                        IQueryable<Drone> currentDronePool, 
                        IAsyncCollector<StatusEntry> statusTable,
                        IAsyncCollector<Drone> newDronePool,
                        CloudQueue readyQueue,
                        CloudQueue requestQueue,
                        TraceWriter log) 
{
    StatusEntry currentStatus = null;
    log.Info($"Incoming request {droneRequest}"); 
    dynamic request = JObject.Parse(droneRequest);

    string assignment = request.Id;
    StatusEntry assigned = new StatusEntry() {
        PartitionKey = "1",
        RowKey = Guid.NewGuid().ToString(),
        Assignment = assignment
    };

    try {
        Drone drone = null;
        foreach(var d in currentDronePool.Where(p => p.Status == "idle").ToList()) {
            drone = d;
            break;
        }
 
        if(drone == null) {
            throw new Exception("No Drones available");
        }
        drone.Status = "en route";
        log.Info($"Drone {drone.Name} selected");
        log.Info($"{newDronePool == null}");
        currentStatus.Drone = drone.RowKey;
        
        request.Add("Assignment", assignment);
        request.Add("Drone", drone.RowKey);

        readyQueue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(request)), null, null, null, null);

        assigned.Drone = drone.RowKey;
        assigned.Event = JsonConvert.SerializeObject(new AssignedEvent() {
            AssignmentId = assignment,
            Status = "Success",
            Drone = drone.RowKey
        });
        await newDronePool.AddAsync(drone);
    }
    catch(Exception ex) {
        log.Error($"Something went wrong: {ex}");

        assigned.Event = JsonConvert.SerializeObject(new AssignedEvent() {
            AssignmentId = assignment, 
            Drone = null,
            Status = $"Failed: {ex}"
        });

        requestQueue.AddMessage(new CloudQueueMessage(droneRequest), null, TimeSpan.FromSeconds(60), null, null);
    }
    finally {
        await statusTable.AddAsync(assigned);
    }         
}

public class Drone : TableEntity {
    public string Name { get; set; }
    public string Status {get; set;}
}


public class StatusEntry : TableEntity {
    public string Assignment { get; set; }
    public string Drone { get; set;}
    public string Event { get; set; }
}

public class AssignedEvent {
    public string AssignmentId { get; set; }
    public string Drone { get; set; }
    public string Status { get; set; }
    public string Type { get { return "assignment"; } }
}

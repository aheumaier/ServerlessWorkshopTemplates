#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
#r "System.Web"
using Microsoft.WindowsAzure.Storage.Table;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System;




private static int EstimateFlightTime(string destination) {
    var rnd = new Random();
    return rnd.Next(15, 60); 
}

public static async Task Run(string readyToFly,  
                        ICollector<StatusEntry> statusTable,
                        IAsyncCollector<NextAction> nextTable,
                        TraceWriter log)
{
    dynamic ready = JObject.Parse(readyToFly);

    log.Info($"Ready to fly: {readyToFly}");

    string assignment = ready.Id;
    string drone = ready.Drone;
    int next = ready.Next;
    string nextLocation = ready.Itiniary[next];

    string _from = next > 0? ready.Itiniary[next - 1]: "_HQ";
    string to = nextLocation;

    int flightTime = EstimateFlightTime(nextLocation);

    HttpClient client = new HttpClient();
    var query = HttpUtility.ParseQueryString(string.Empty);
    query["id"] = ready.Assignment;
    query["url"] = "https://dronebiz.azurewebsites.net/api/flight";
    query["flightTime"] = flightTime.ToString();

    var builder = new UriBuilder("https://dronebiz.azurewebsites.net/api/RegisterFlight");
    builder.Query = query.ToString(); 

    var _event = new StatusEntry() {
        PartitionKey = "1",
        RowKey = Guid.NewGuid().ToString(),
        Drone = drone,
        Assignment = assignment
    };
    try {
        var response = await client.GetAsync(builder.ToString());
       
        _event.Event = JsonConvert.SerializeObject(new LiftOffEvent() {
                From = _from,
                To = to, 
                EstimatedFlightTime = flightTime,
                Status = "Success"
            });
        await nextTable.AddAsync(new NextAction() {
            PartitionKey = "liftoff",
            RowKey = assignment,
            Drone = drone,
            Next = next + 1,
            Itiniary = ready.Itiniary.Join(",")
        });
    }
    catch(Exception ex) {
        log.Error($"Something went wrong: {ex}");

        _event.Event = JsonConvert.SerializeObject(new LiftOffEvent() {
                From = _from,
                To = to, 
                EstimatedFlightTime = flightTime,
                Status = $"Failed: {ex}"
            });
    }
    finally {
        statusTable.Add(_event);
    }         

}


public class StatusEntry : TableEntity {
    public string Assignment { get; set; }
    public string Drone { get; set;}
    public string Event { get; set; }
}

public class LiftOffEvent { 
    public string From { get; set; }
    public string To { get; set; }
    public int EstimatedFlightTime { get; set; }
    public string Type { get {return "liftoff";}}
    public string Status { get; set; }
}

public class NextAction: TableEntity {
    public string Drone { get; set; }
    public int Next { get; set; }
    public string Itiniary { get; set; }
}

#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;


using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req,IAsyncCollector<Flight> flights, TraceWriter log)
{
    // parse query
    var parameters = req.GetQueryNameValuePairs();
    string drone = parameters 
        .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
        .Value;
    string url = parameters 
        .FirstOrDefault(q => string.Compare(q.Key, "url", true) == 0)
        .Value;
    int totalFlightTime = Int32.Parse(parameters
        .FirstOrDefault(q => string.Compare(q.Key, "flightTime", true) == 0)
        .Value);

    if(String.IsNullOrEmpty(drone) || String.IsNullOrEmpty(url) || totalFlightTime <= 0)
        return req.CreateResponse(HttpStatusCode.BadRequest);
    
    Uri _url;

    try {
        _url = new Uri(url);
    } catch(Exception ex) {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    await flights.AddAsync(new Flight() {
        PartitionKey = "1",
        RowKey = Guid.NewGuid().ToString(),
        Url = _url.ToString(),
        AssociatedId = drone,
        TotalFlightTime = totalFlightTime,
        RemainingFlightTime = totalFlightTime
    });

    return req.CreateResponse(HttpStatusCode.OK,"");
}


public class Flight: TableEntity {
    public int TotalFlightTime { get; set; }
    public int RemainingFlightTime { get; set; } 
    public string AssociatedId { get; set; }
    public string Url { get; set; }
}
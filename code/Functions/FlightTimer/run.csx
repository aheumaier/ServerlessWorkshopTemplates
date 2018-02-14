#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"
#r "System.Web"
using Microsoft.WindowsAzure.Storage.Table;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System;

public static async Task Run(TimerInfo schedule,  IQueryable<Flight> flights, ICollector<Flight> newFlights, TraceWriter log)
{ 
    HttpClient client = new HttpClient();
    foreach(var f in flights.Where(p => p.RemainingFlightTime > 0).ToList()) { 
        log.Info($"{f.ToString()}");
        f.RemainingFlightTime--;
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["id"] = f.AssociatedId;
        query["remainingFlightTime"] = f.RemainingFlightTime.ToString();
        
        var builder = new UriBuilder(f.Url);
        builder.Query = query.ToString(); 

        var response = await client.GetAsync(builder.ToString());

        try {
            response.EnsureSuccessStatusCode();
            newFlights.Add(f); 
        }
        catch (HttpRequestException)
        {
            log.Error($"Could not fetch: {builder.ToString()}");
        }
    }
}


public class Flight: TableEntity {
    public int TotalFlightTime { get; set; }
    public int RemainingFlightTime { get; set; } 
    public string AssociatedId { get; set; }
    public string Url { get; set; }
}
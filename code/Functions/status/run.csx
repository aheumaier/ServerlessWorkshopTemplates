#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using Newtonsoft.Json;

using Newtonsoft.Json.Linq;

using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req,  IQueryable<StatusEntry> statusTable, TraceWriter log)
{

    // parse query parameter
    string id = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
        .Value;
    log.Info($"Status on assignment '{id}' requested.");  
    var events = new List<JObject>();
    if(!String.IsNullOrEmpty(id)) {
        try {
           
            foreach(var entry in statusTable.Where(p => p.Assignment == id).ToList()) {
                var obj = new JObject();  
                obj.Add(entry.Timestamp.ToString(), JObject.Parse(entry.Event));
                events.Add(obj);
            };
        }
        catch(Exception ex)
        {
            log.Error($"Invalid assignment id: {id}");
        }
    }
    
    string content = events == null? "{\"error\": \"No status available\"}": JsonConvert.SerializeObject(events);
    return new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(content, Encoding.UTF8, "application/json")
    };
}

public class StatusEntry : TableEntity
{
    public string Assignment { get; set; }
    public string Drone { get; set; }
    public string Event { get; set; }

}
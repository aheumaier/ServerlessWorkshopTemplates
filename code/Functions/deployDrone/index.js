function uuidv4() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}


module.exports = function (context, req, currentDronePool) {
    context.log("Deploying new drones ...");
    if(currentDronePool.length > 0)
    {
        context.res = {
            status: 400, 
            body: {
                error: "Drones are already deployed."
            }
        }
    }
    else 
    {
        let n = parseInt(req.query.n);
        if (n && n > 0) {
        let drones = [...Array(n).keys()].map((i) => {
            let key = uuidv4();
            return {
                "partitionKey": "1",
                "rowKey": key,
                "Name": "Drone " + i, 
                "Status": "idle"
            };
        });
        
        context.bindings.dronePool = drones;
        context.res = {
            status: 200,
            body: {
                drones: drones
            }
        }
        }
        else {
            context.res = {
                status: 400,
                body: {
                    error: "Invalid parameters."
                }
            };
        }
    }
    context.done();
};


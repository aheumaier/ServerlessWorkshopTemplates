function uuidv4() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

module.exports = function (context, req) {
    if (req.query.from && req.query.to) {
        let from_loc = req.query.from;
        let to_loc = req.query.to;
        let request_id = uuidv4();
        context.log("Processing drone request from " + from_loc + " to " + to_loc + ". Id " + request_id);


        let request = {
            Id: request_id,
            Itiniary: [ from_loc, to_loc ],
            Next: 0,
        };

        context.bindings.requestQ = request;
        context.log("Added drone request to queue");
        
        context.bindings.statusTable = {
            rowKey: uuidv4(),
            partitionKey: 123,
            Assignment: request_id,
            Drone: null,
            Event: JSON.stringify({
                when: Date.now(),
                type: "requested"
            })  
        };
        context.res = {
            // status: 200, /* Defaults to 200 */
            body: request
        };
    }
    else {
        context.res = {
            status: 400,
            body: {
                error: "Parameters not supplied."
            }
        };
    }
    context.done();
};
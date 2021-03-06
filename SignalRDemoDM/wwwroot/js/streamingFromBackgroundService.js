"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/streamingFromBackgroundServiceHub")
    //.configureLogging(signalR.LogLevel.Information)
    // very useful for seeing connection issues in chrome dev tools
    .configureLogging(signalR.LogLevel.Trace)
    .build();

connection.start()
    .then(function () {
        console.log("SignalR Connected - logging from javascript");

        // connection established so can start the stream by a fake button click
        var crawlButton = document.getElementById("crawlButton");
        crawlButton.click();
    })
    .catch(function (err) {
        return console.error(err.toString());
    });


// ReceiveMessage that is called by the Hub (BackgroundService now!)
connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});


document.getElementById("crawlButton").addEventListener("click", function (event) {
    // need subscription so can dispose (cancel) below
    var subscription = connection.stream("Counter", 5, 500)
        .subscribe({
            next: (item) => {
                var li = document.createElement("li");
                li.textContent = item;
                document.getElementById("messagesList").appendChild(li);
            },
            complete: () => {
                var li = document.createElement("li");
                li.textContent = "Stream completed";
                document.getElementById("messagesList").appendChild(li);
            },
            error: (err) => {
                var li = document.createElement("li");
                li.textContent = err;
                document.getElementById("messagesList").appendChild(li);
            },
        });

    // wire up the stop button
    // calling dispose causes cancellation on the server
    // https://docs.microsoft.com/en-us/aspnet/core/signalr/streaming?view=aspnetcore-3.0#server-to-client-streaming-2
    document.getElementById("cancelButton").addEventListener("click", function (event) {
        subscription.dispose();
        // update the UI as no messages come back from server once cancelled
        var li = document.createElement("li");
        li.textContent = "Cancelled (message made from js)";
        document.getElementById("messagesList").appendChild(li);
    });


});



//document.getElementById("throwNormalExceptionButton").addEventListener("click", function (event) {
//    connection.invoke("ThrowNormalException")
//        .catch(function (err) {
//            return console.error(err.toString());
//        });
//    event.preventDefault();
//});

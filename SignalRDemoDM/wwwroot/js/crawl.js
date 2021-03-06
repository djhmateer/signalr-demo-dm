"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/crawlHub")
    //.configureLogging(signalR.LogLevel.Information)
    // very useful for seeing connection issues in chrome dev tools
    .configureLogging(signalR.LogLevel.Trace)
    .build();

connection.start()
    .then(function () {
        console.log("SignalR Connected - logging from javascript");
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

document.getElementById("crawlButton").addEventListener("click", function (event) {
    var urlToCrawl = document.getElementById("urlToCrawl").value;
    // need subscription so can dispose (cancel) below
    var subscription = connection.stream("Crawl", urlToCrawl)
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

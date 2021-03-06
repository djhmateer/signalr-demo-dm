"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/fooHub")
    //.configureLogging(signalR.LogLevel.Information)
    // very useful for seeing connection issues in chrome dev tools
    .configureLogging(signalR.LogLevel.Trace)
    .build();

connection.start()
    .then(function () {
        //document.getElementById("sendButton").disabled = false;
        console.log("SignalR Connected - logging from javascript");
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

// Javascript ReceiveMessage function that is called by the Hub
connection.on("ReceiveMessageFoo", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    //call the Hub method SendMessageToCaller
    connection.invoke("SendMessageToCaller", user, message)
        .catch(function (err) {
            return console.error(err.toString());
        });
    event.preventDefault();
});


document.getElementById("throwExceptionButton").addEventListener("click", function (event) {
    connection.invoke("ThrowException")
        .catch(function (err) {
            return console.error(err.toString());
        });
    event.preventDefault();
});


document.getElementById("throwNormalExceptionButton").addEventListener("click", function (event) {
    connection.invoke("ThrowNormalException")
        .catch(function (err) {
            return console.error(err.toString());
        });
    event.preventDefault();
});

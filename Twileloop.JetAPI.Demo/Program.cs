﻿using System.Text.Json;
using Twileloop.JetAPI;

await PUT_WithObjectAsJSONBody();

//Default GET API
static async Task GET_Default() {
    var response = await new JetRequest()
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts/1");
    PrintResponse(response);
}

//GET with query parameters
static async Task GET_WithQueryParameters() {
    var response = await new JetRequest()
                            .WithQueries(
                                new Param("postId", 2)
                            )
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/comments");
    PrintResponse(response);
}

//Get with header parameters
static async Task GET_WithHeaders() {
    var response = await new JetRequest()
                            .WithQueries(
                                new Param("postId", 3)
                            )
                            .WithHeaders(
                                new Param("x-request-by", "jetTask"),
                                new Param("x-maxlimit", 150)
                            )
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/comments");
    PrintResponse(response);
}

//Post with plain JSON text body
static async Task POST_WithJSONString() {

    var jsonString = @"{""title"":""Foo"",""bar"":""Bar"",""userid"":1}";

    //In this case => RawBody assumes you're gigin the data that's of type JSON
    var response = await new JetRequest()
                            .Post()
                            .WithBody(
                                new RawBody(BodyType.Json, jsonString) 
                            )
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts");
    PrintResponse(response);
}

//Post with object as JSON body
static async Task POST_WithObjectAsJSONBody() {

    var instance = new {
        Title = "Foo",
        Bar = "Bar",
        UserId = 1
    };

    //In this case => RawBody assumes you want to serialize this object as JSON
    var response = await new JetRequest()
                            .Post()
                            .WithBody(
                                new RawBody(BodyType.Json, instance)
                            )
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts");
    PrintResponse(response);
}


//Put with object as JSON body
static async Task PUT_WithObjectAsJSONBody() {

    var instance = new {
        Title = "Foo",
        Bar = "Bar",
        UserId = 1
    };

    var response = await new JetRequest()
                            .Put()
                            .WithBody(
                                new RawBody(BodyType.Json, instance)
                            )
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts/1");
    PrintResponse(response);
}








//Common function to print to console
static void PrintResponse(dynamic response) {
    Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions {
        WriteIndented = true
    }));
    Console.ReadLine();
}
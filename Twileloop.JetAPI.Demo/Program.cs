using System.Net;
using System.Text.Json;
using Twileloop.JetAPI;
using Twileloop.JetAPI.Authentication;
using Twileloop.JetAPI.Body;
using Twileloop.JetAPI.Demo;
using Twileloop.JetAPI.Types;

await GET_WithExtendedCaptures();

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


//With Basic Authentication
static async Task GET_WithBasicAuthentication() {

    var response = await new JetRequest()
                            .Get()
                            .WithAuthentication(new BasicAuthentication {
                                Username = "username",
                                Password = "password",
                                EncodeAsBase64 = true
                            })
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//With API_KEY Authentication
static async Task GET_WithAPIKEYAuthentication() {

    var response = await new JetRequest()
                            .Get()
                            .WithAuthentication(new ApiKey("Api-Key", "<API_KEY>"))
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//With Bearer Authentication
static async Task GET_WithBearerAuthentication() {

    var response = await new JetRequest()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}



//With Interceptors
static async Task GET_WithInterceptor() {

    var interceptor = new CustomInterceptor();

    var response = await new JetRequest()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .WithInterceptor(interceptor)
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//On Exceptions
static async Task GET_HandleExceptions() {

    var response = await new JetRequest()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .HandleExceptions(
                                ex => {
                                    Console.WriteLine($"An exception occured. Message: {ex.Message}");
                                }
                            )
                            .ExecuteAsync<dynamic>("htt://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//With Captures
static async Task GET_WithCaptures() {

    var response = await new JetRequest()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .WithCaptures(
                                () => {
                                    Console.WriteLine("Success");
                                },
                                () => {
                                    Console.WriteLine("Failure");
                                }
                            )
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//With extended Captures
static async Task GET_WithExtendedCaptures() {

    var response = await new JetRequest()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .WithCaptures(
                                (HttpStatusCode.OK, () => Console.WriteLine("Ok")),
                                (HttpStatusCode.NotFound, () => Console.WriteLine("Not Found")),
                                (HttpStatusCode.Unauthorized, () => Console.WriteLine("UnAuthorized")),
                                (HttpStatusCode.Forbidden, () => Console.WriteLine("Forbidden"))
                            )
                            .ExecuteAsync<dynamic>("https://jsonplaceholder.typicode.com/fake");
    PrintResponse(response);
}

//Common function to print to console
static void PrintResponse(dynamic response) {
    Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions {
        WriteIndented = true
    }));
    Console.ReadLine();
}
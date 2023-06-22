using System.Net;
using System.Text.Json;
using Twileloop.JetAPI;
using Twileloop.JetAPI.Authentication;
using Twileloop.JetAPI.Body;
using Twileloop.JetAPI.Demo;
using Twileloop.JetAPI.DemoServer;
using Twileloop.JetAPI.Types;



const string BASE_URL = "https://localhost:7177/WeatherForecast";

//Default Get
var res1 = await new JetRequest<dynamic>()
                     .ExecuteAsync($"{BASE_URL}");

//Get with query prams
var res2 = await new JetRequest<dynamic>()
                     .WithQueries(
                        new Param("fname", "Sangeeth"),
                        new Param("lname", "Nandakumar")
                     )
                     .ExecuteAsync($"{BASE_URL}");

//Get with header params
var res3 = await new JetRequest<dynamic>()
                     .WithHeaders(
                        new Param("header1", "Sangeeth"),
                        new Param("header2", "Nandakumar")
                      )
                     .ExecuteAsync($"{BASE_URL}/HeaderTest");


//Get with cookie parameter
var res4 = await new JetRequest<dynamic>()
                     .WithCookies(
                        new Param("fname", "Sangeeth"),
                        new Param("lname", "Nandakumar")
                      )
                     .ExecuteAsync($"{BASE_URL}/CookieTest");

//BODY

//With FORM body
var res5 = await new JetRequest<dynamic>()
                     .Post()
                     .WithFormData(
                        new Param("fname", "Sangeeth"),
                        new Param("lname", "Nandakumar")
                      )
                     .ExecuteAsync($"{BASE_URL}/FormTest");

//With RAW body
var res6 = await new JetRequest<dynamic>()
                     .Post()
                     .WithBody(
                        new RawBody(ContentType.Json, new WeatherForecast
                        {
                            Date = DateOnly.Parse("28-10-1996"),
                            Summary = "Summry",
                            TemperatureC = 32,
                        })
                     )
                     .ExecuteAsync($"{BASE_URL}");

var res7 = await new JetRequest<dynamic>()
                     .Post()
                     .WithBody(
                        new RawBody(ContentType.Json, JsonSerializer.Serialize(
                            new WeatherForecast
                            {
                                Date = DateOnly.Parse("28-10-1996"),
                                Summary = "Summary",
                                TemperatureC = 32,
                            })
                        )
                     )
                     .ExecuteAsync($"{BASE_URL}");







//With Basic Authentication
static async Task GET_WithBasicAuthentication() {

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .WithAuthentication(new BasicAuthentication {
                                Username = "username",
                                Password = "password",
                                EncodeAsBase64 = true
                            })
                            .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//With API_KEY Authentication
static async Task GET_WithAPIKEYAuthentication() {

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .WithAuthentication(new ApiKey("Api-Key", "<API_KEY>"))
                            .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//With Bearer Authentication
static async Task GET_WithBearerAuthentication() {

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}



//With Interceptors
static async Task GET_WithInterceptor() {

    var interceptor = new CustomInterceptor();

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .WithInterceptor(interceptor)
                            .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//On Exceptions
static async Task GET_HandleExceptions() {

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .HandleExceptions(
                                ex => {
                                    Console.WriteLine($"An exception occured. Message: {ex.Message}");
                                }
                            )
                            .ExecuteAsync("htt://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//With Captures
static async Task GET_WithCaptures() {

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .WithCaptures(
                                successResponse => {
                                    Console.WriteLine("Success");
                                },
                                failureResponse => {
                                    Console.WriteLine("Failure");
                                }
                            )
                            .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
    PrintResponse(response);
}

//Request and response cookies
static async Task GET_WithCookies() {

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .WithCookies(
                                new Param("Cookie1", "<CookieValue1>"),
                                new Param("Cookie2", "<CookieValue2>")
                             )
                            .FetchAs(ContentType.HTML)
                            .ExecuteAsync("https://google.com");
    PrintResponse(response);
}

//With extended Captures
static async Task GET_WithExtendedCaptures() {

    var response = await new JetRequest<dynamic>()
                            .Get()
                            .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                            .WithCaptures(
                                (HttpStatusCode.OK, () => Console.WriteLine("Ok")),
                                (HttpStatusCode.NotFound, () => Console.WriteLine("Not Found")),
                                (HttpStatusCode.Unauthorized, () => Console.WriteLine("UnAuthorized")),
                                (HttpStatusCode.Forbidden, () => Console.WriteLine("Forbidden"))
                            )
                            .ExecuteAsync("https://jsonplaceholder.typicode.com/fake");
    PrintResponse(response);
}

//Common function to print to console
static void PrintResponse(dynamic response) {
    Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions {
        WriteIndented = true
    }));
    Console.ReadLine();
}
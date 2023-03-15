<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/sangeethnandakumar/Jet API">
    <img src="https://github.com/sangeethnandakumar/Twileloop.JetAPI/raw/master/Twileloop.JetAPI/logo.png" alt="Logo" width="80" height="80">
  </a>

  <h2 align="center"> Jet API </h2>
  <h4 align="center"> Free | Open-Source | Fast </h4>

  <p align="center">
    <b> 
			An easy to use conveinent fluent API requester for all your personal or commercial .NET apps
		 </b>
    <br />
    <a href="https://twileloop.epub.readthedocs.io"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/sangeethnandakumar/Twileloop.EPub/issues">Report A Bug</a>
    ·
    <a href="https://github.com/sangeethnandakumar/Twileloop.EPub/issues">Request A Feature</a>
  </p>
  
</div>

## SonarCloud
* Continues security & maintanance scans on every bug fixes and features
* SonarCloud: [Click Here For SonarCloud Report](https://sonarcloud.io/summary/new_code?id=sangeethnandakumar_Twileloop.JetAPI). 

[![.NET](https://img.shields.io/github/actions/workflow/status/sangeethnandakumar/Twileloop.JetAPI/sonarscan.yml?branch=master&label=SonarQube%20%28Code%20Quality%29&style=flat-square)](https://sonarcloud.io/summary/new_code?id=sangeethnandakumar_Twileloop.JetAPI)

## Fully Free
* MIT Licensed
* Fully free for personal/commercial projects without needing any commertial licenses or messes.

## Feeling happy? A small coffee would be a great way to support my work. Thank you for considering it!
[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/sangeethnanda)


<div align="center">

  <h2 align="center"> DOCUMENTATION </h2>
  <h4 align="center"> A fully free open source cross platform fluent APIClient for your .NET application </h4>
  
</div>

# Simple GET

```csharp
var response = await new JetRequest<dynamic>()
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/1");
```

# GET 🠚 With Query Params

```csharp
var response = await new JetRequest<dynamic>()
                         .WithQueries(
                             new Param("postId", 2),
                             new Param("date", "1996-10-28")
                         )
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/comments");```
```

# GET 🠚 With Headers

```csharp
var response = await new JetRequest<dynamic>()
                         .WithQueries(
                             new Param("postId", 3)
                         )
                         .WithHeaders(
                             new Param("x-request-by", "name"),
                             new Param("x-limit", 150)
                         )
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/comments");                         .ExecuteAsync("https://jsonplaceholder.typicode.com/comments");```
```

# POST 🠚 With JSON String

```csharp
var jsonString = @"{""title"":""Foo"",""bar"":""Bar"",""userid"":1}";

var response = await new JetRequest<dynamic>()
                        .Post()
                        .WithBody(
                            new RawBody(ContentType.Json, jsonString)
                        )
                        .ExecuteAsync("https://jsonplaceholder.typicode.com/posts");
```

# PUT 🠚 With Object As JSON

```csharp
var instance = new {
    Title = "Foo",
    Bar = "Bar",
    UserId = 1
};

var response = await new JetRequest<MyResponseModel>()
                        .Put()
                        .WithBody(
                            new RawBody(ContentType.Json, instance)
                        )
                        .ExecuteAsync("https://jsonplaceholder.typicode.com/posts");
```


# GET 🠚 With Basic-Authentication

```csharp
var response = await new JetRequest<MyResponseModel>()
                         .Get()
                         .WithAuthentication(new BasicAuthentication {
                             Username = "username",
                             Password = "password"
                         })
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
```

# GET 🠚 With JWT Bearer-Authentication

```csharp
var response = await new JetRequest<MyResponseModel>()
                         .Get()
                         .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
```

# GET 🠚 With API_KEY Authentication

```csharp
var response = await new JetRequest<MyResponseModel>()
                         .Get()
                         .WithAuthentication(new ApiKey("Api-Key", "<API_KEY>"))
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
```

# PATCH 🠚 And Handle Exceptions Yourself

```csharp
var response = await new JetRequest<MyResponseModel>()
                         .Patch()
                         .HandleExceptions(
                             ex => Console.WriteLine($"An exception occured. Message: {ex.Message}");
                         )
                         .ExecuteAsync("htt://jsonplaceholder.typicode.com/posts/5");
```

# GET 🠚 With Success/Failure Captures

```csharp
var response = await new JetRequest<MyResponseModel>()
                         .Get()
                         .WithCaptures(
                             successResponse => Console.WriteLine("Success");,
                             failureResponse => Console.WriteLine("Failure");
                         )
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
```

# PUT 🠚 With Custom Captures Based On HTTP StatusCode

```csharp
var response = await new JetRequest<MyResponseModel>()
                         .Put()
                         .WithCaptures(
                             (HttpStatusCode.OK, () => Console.WriteLine("Ok")),
                             (HttpStatusCode.NotFound, () => Console.WriteLine("Not Found")),
                             (HttpStatusCode.Unauthorized, () => Console.WriteLine("UnAuthorized")),
                             (HttpStatusCode.Forbidden, () => Console.WriteLine("Forbidden"))
                         )
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/fake");
```

# GET 🠚 As JSON/XML/HTML or TEXT

```csharp
var response = await new JetRequest<MyResponseModel>()
                          .Get()
                          .FetchAs(ContentType.XML)
                          .ExecuteAsync("https://samplexml.com/auth/demoxml.xml");
```

# GET 🠚 And Pass Request Cookies

```csharp
var response = await new JetRequest<MyResponseModel>()
                          .WithCookies(
                              new Param("Cookie1", "<CookieValue1>"),
                              new Param("Cookie2", "<CookieValue2>")
                           )
                          .FetchAs(ContentType.HTML)
                          .ExecuteAsync("https://google.com");
```

# Listen To Events With Interceptors

Create your own intercepter by inheriting from Interceptor base class
```csharp
public class CustomInterceptor : Interceptor {

        public override void OnInit() {
            Console.WriteLine("Started...");
            base.OnInit();
        }

        public override void OnRequesting(Request request) {
            Console.WriteLine("Let's modify request from interceptor");
            request.HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            Console.WriteLine("Enough. Now start requesting...");
            base.OnRequesting(request);
        }

        public override void OnResponseReceived() {
            Console.WriteLine("Got response...");
            base.OnResponseReceived();
        }

    }
```

Do your stuff above. Alter anything before request goes or log result after response came etc.. Now let's attach this interceptor to JetAPI

```csharp
var interceptor = new CustomInterceptor();

 var response = await new JetRequest<dynamic>()
                         .Post()
                         .WithAuthentication(new BearerToken("<BEARER_TOKEN>"))
                         .WithInterceptor(interceptor)
                         .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/5");
```
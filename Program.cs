var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 1. Register the HttpClient service for making external API calls
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Allows CSS/JS files to be served
app.UseRouting();
app.UseAuthorization();

// 2. Map the Razor Pages (for the Index.cshtml file)
app.MapRazorPages();

// 3. Add the Minimal API endpoint to fetch a joke
app.MapGet("/getjoke", async (IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient();
    try
    {
        // Call the external Joke API
        var response = await client.GetAsync("https://official-joke-api.appspot.com/random_joke");

        if (response.IsSuccessStatusCode)
        {
            // Send the raw JSON (containing 'setup' and 'punchline') to the frontend
            var jokeJson = await response.Content.ReadAsStringAsync();
            return Results.Content(jokeJson, "application/json");
        }

        return Results.Problem("Could not fetch a joke right now.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred: {ex.Message}");
    }
});


app.Run();
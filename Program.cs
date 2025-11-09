using LostAndFoundWebUi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<LostAndFoundApiService>(client =>
{
    // Configure the base address for your API.
    // IMPORTANT: Replace the URL with the correct URL/port your API is running on.
    // For development, this is typically https://localhost:<PortNumber>
    // NOTE: Ensure you append the trailing slash for correct relative path resolution later (e.g., "Auth/login")
    client.BaseAddress = new Uri("https://localhost:44377/api/"); // Local development



});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 🌟 CHANGE 1: Move app.UseSession() before app.UseAuthorization() and app.MapRazorPages()
// Session must be configured before it is accessed.
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

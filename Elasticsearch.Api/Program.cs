var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region [ELASTÝCSEARCH]

builder.Services.AddElasticsearchClient_ElasticClients(builder.Configuration);

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

#endregion [ELASTÝCSEARCH]

#region [AUTO_MAPPER]

var assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                                                .Where(filePath => Path.GetFileName(filePath)
                                                .StartsWith("Elasticsearch"))
                                                .Select(Assembly.LoadFrom);
builder.Services.AddAutoMapper(assemblies);

#endregion [AUTO_MAPPER]

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
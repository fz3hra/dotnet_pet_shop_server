using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using dotnet_pet_api.Models;
using DbaseContext;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var sqlConBuilder = new SqlConnectionStringBuilder();

var connectionString = builder.Configuration.GetConnectionString("PetsDb") ?? "Data Source=Pets.db";
sqlConBuilder.UserID = builder.Configuration["UserId"];
sqlConBuilder.Password = builder.Configuration["Password"];


builder.Services.AddEndpointsApiExplorer();

// in memory database:
//builder.Services.AddDbContext<PetsDb>(options => options.UseInMemoryDatabase("items"));

// connecting to sqlite:
//builder.Services.AddSqlite<PetsDb>(connectionString);


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PetAPi API", Description = "So that you can find pets that you like", Version = "v1" });
});

//conecting to sql server:
builder.Services.AddDbContext<PetsDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetAPi API V1");
});

app.MapGet("/", () => "Hello World!");

app.MapGet("/pets", async (PetsDb database) => await database.Categories.ToListAsync());

app.MapPost("/createPets", async (PetsDb database, Categories categories) =>
{
    await database.Categories.AddAsync(categories);
    await database.SaveChangesAsync();
    return Results.Created($"/createPets/{categories.Id}", categories);
});

/////////////////////////////////////////////
///

app.MapGet("/getTodos", async (PetsDb database) => await database.CreateToDo.ToListAsync());


// START: CREATE TO DO API
app.MapPost("/createTodos", async (PetsDb database, CreateToDo createToDo) =>
{
    await database.CreateToDo.AddAsync(createToDo);
    await database.SaveChangesAsync();
    return Results.Created($"/createTodos/{createToDo.Id}", createToDo);
});
// END: CREATE TO DO API

app.MapPut("/updateTodo/{id}", async (PetsDb database, CreateToDo updateTodo, int id) =>
{
    var category = await database.CreateToDo.FindAsync(id);
    if (category is null) return Results.NotFound();
   // category.Title = updateTodo.Title;
    category.isChecked = updateTodo.isChecked;
    await database.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/deleteTodo/{id}", async (PetsDb db, int id) =>
{
    var toDo = await db.CreateToDo.FindAsync(id);
    if (toDo is null)
    {
        return Results.NotFound();
    }
    db.CreateToDo.Remove(toDo);
    await db.SaveChangesAsync();
    return Results.Ok();
});
/////////////////////////////////////////////

app.MapGet("/getPetId/{id}", async (PetsDb db, int id) => await db.Categories.FindAsync(id));

app.MapPut("/updatePet/{id}", async (PetsDb database, Categories updateCategory, int id) =>
{
    var category = await database.Categories.FindAsync(id);
    if (category is null) return Results.NotFound();
    category.CategoryName = updateCategory.CategoryName;
    await database.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/deletePet/{id}", async (PetsDb db, int id) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category is null)
    {
        return Results.NotFound();
    }
    db.Categories.Remove(category);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// !creating a user
app.MapPost("/createUser/", async (PetsDb database, Users users) =>
{
    // check if user already exists:
    var existingUser = database.Users.Where(eUser => eUser.FullName == users.FullName && eUser.EmailAddress == users.EmailAddress).FirstOrDefault();
    if (existingUser != null)
    {
        return Results.BadRequest("User already exist under the following name and email address");
    }
    // 
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
    const int keySize = 64;
    const int iterations = 350000;
    var salt = RandomNumberGenerator.GetBytes(keySize);
    var hash = Rfc2898DeriveBytes.Pbkdf2(
        Encoding.UTF8.GetBytes(users.Password),
        salt,
        iterations,
        hashAlgorithm,
        keySize);
    users.Password = Convert.ToHexString(hash);
    // users.SaltedPassword = Convert.ToHexString(salt);
    await database.Users.AddAsync(users);
    await database.SaveChangesAsync();
    return Results.Created($"/createUser/{users.Id}", users);
});

// ! logging a user 
app.MapPost("/loginUser/", async (PetsDb database, Users user) =>
{
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
    // find if the user is in the database and if password matches


    var findUser = database.Users.Where(u => u.FullName == user.FullName).FirstOrDefault();
    if (findUser == null)
    {
        return Results.BadRequest("no user");
    }
    bool VerifyPassword(string password, string hash, byte[] salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2("password", salt, 350000, hashAlgorithm, 64);
        return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
    }
    return Results.Ok(findUser);
    // else return token along with other details.
});

// ! getting users by id


app.Run();


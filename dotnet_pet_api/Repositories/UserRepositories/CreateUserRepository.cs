// using DbaseContext;
// using dotnet_pet_api.Models;

// namespace DOTNET_PET_API.Repositories
// {
//     class CreateUserRepository
//     {
//         public async createUser(PetsDb database, Users users)
//         {
//             await database.Users.AddAsync(users);
//             await database.SaveChangesAsync();
//             return Results.Created($"/createUser/{users.Id}", users);
//         }
//     }
// }
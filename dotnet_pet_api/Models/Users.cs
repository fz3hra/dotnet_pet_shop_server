using Microsoft.EntityFrameworkCore;

namespace dotnet_pet_api.Models
{
    public class Users
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String? EmailAddress { get; set; }
        public String Password { get; set; }
    }
}

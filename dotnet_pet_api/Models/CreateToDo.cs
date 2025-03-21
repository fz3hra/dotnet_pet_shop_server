using System;
namespace dotnet_pet_api.Models
{
	public class CreateToDo
	{
		public int Id { get; set; }
        public String Title { get; set; }
        public Boolean isChecked { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Dtos
{
    public class UserDto
    {

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }



    }
}

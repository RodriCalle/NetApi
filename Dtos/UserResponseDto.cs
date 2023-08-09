namespace Supermarket.API.Resources.Dtos
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        public string Token { get; set; }
    }
}

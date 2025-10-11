namespace ElectroKart_Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeRoleAttribute : Attribute
    {
        public string Role { get; }

        public AuthorizeRoleAttribute(string role)
        {
            Role = role;
        }
    }
}
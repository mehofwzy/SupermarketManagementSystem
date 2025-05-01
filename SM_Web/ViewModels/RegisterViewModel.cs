using System.ComponentModel.DataAnnotations;

namespace SM_Web.ViewModels
{
    public class RegisterViewModel
    {
        
        public Guid Id { get; set; }
        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public Guid CreatedById { get; set; }

    }
}

using System;

namespace CloudPic.Models.DTO
{
    public class User
    {
        public User()
        {
        }

        public User(int id, string email, string password, string name, string loginProvider,DateTime registerDate)
        {
            Id = id;
            Email = email;
            Password = password;
            Name = name;
            LoginProvider = loginProvider;
            RegisterDate = registerDate;
        }

        public User(string email, string password, string name, string loginProvider, DateTime registerDate)
            : this (0, email, password, name, loginProvider, registerDate)
        {
            
        }

        public int Id { get; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public string LoginProvider { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}

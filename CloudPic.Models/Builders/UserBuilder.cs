using CloudPic.Models.DTO;
using CloudPic.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.Builders
{
    public class UserBuilder : IBuilder<User>
    {
        private readonly User user;

        public UserBuilder()
        {
            this.user = new User
            {
                Password = "",
                LoginProvider = LoginProvider.Local.ToString(),
            };
        }

        public UserBuilder SetEmail(string email)
        {
            this.user.Email = email;
            return this;
        }

        public UserBuilder SetPassword(string password)
        {
            this.user.Password = password;
            return this;
        }

        public UserBuilder SetName(string name)
        {
            this.user.Name = name;
            return this;
        }

        public UserBuilder SetLoginProvider(LoginProvider loginProvider)
        {
            this.user.LoginProvider = loginProvider.ToString();
            return this;
        }

        public User Build()
        {
            this.user.RegisterDate = DateTime.Now;
            return this.user;
        }
    }
}

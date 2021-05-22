using CloudPic.DAL.Concretes;
using CloudPic.DAL.Interfaces;
using System;

namespace CloudPic.DAL
{
    public static class RepoFactory
    {
        private static UserRepo userRepo;
        public static IUserRepo GetUserRepo() => userRepo ??= new UserRepo();

    }
}

using CloudPic.DAL.Concretes;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.BAL.Patterns.Factory
{
    public static class FileShareFactory
    {
        public static IFSRepo GetFSRepo(FSType fSType, IConfiguration configuration)
        {
            return fSType switch
            {
                FSType.AzureFS => new FSRepo(configuration),
                _ => throw new NotImplementedException(),
            };
        }
    }
}

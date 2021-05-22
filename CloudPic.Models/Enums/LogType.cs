using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.Enums
{
    public enum LogType
    {
        LoggedIn = 1,
        LoggedOut = 2,
        ChangedPackage = 3,
        UploadedPhoto = 4,
        DeletedPhoto = 5,
        EditedPhoto = 6,
        DeletedUser = 7
    }
}

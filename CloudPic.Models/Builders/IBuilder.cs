using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.Builders
{
    public interface IBuilder<T>
    {
        T Build();
    }
}

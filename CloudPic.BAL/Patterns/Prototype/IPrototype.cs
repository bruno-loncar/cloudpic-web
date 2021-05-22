using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.BAL.Prototype
{
    public interface IPrototype<T>
    {
        T Clone();
    }
}

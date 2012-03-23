using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public interface IControlPoint2f : INotifyPropertyChanged
    {
        Vector2f Position { get; set; }
        bool IsCorner { get; set; }

        IControlPoint2f Clone();
    }
}

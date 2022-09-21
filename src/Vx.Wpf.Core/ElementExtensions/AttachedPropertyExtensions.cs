using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vx.Wpf.Core.ElementExtensions
{
    public static class AttachedPropertyExtensions
    {
        public static T AttachProperties<T>(this T vxElement, Action<UIElement> action) where T : VxElement
        {
            vxElement.AttachProperties(action);

            return vxElement;
        }
    }
}

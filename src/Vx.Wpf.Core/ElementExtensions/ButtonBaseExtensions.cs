using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Vx.Wpf
{
    public static class ButtonBaseExtensions
    {
        public static T VxClick<T>(this T buttonBase, Action onClick) where T : ButtonBase
        {
            buttonBase.Click += delegate
            {
                onClick();
            };

            return buttonBase;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Vx.Wpf
{
    public static class GridExtensions
    {
        public static T GridColumn<T>(this T el, int column) where T : FrameworkElement
        {
            Grid.SetColumn(el, column);
            return el;
        }
    }
}

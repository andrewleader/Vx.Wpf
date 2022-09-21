using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Vx.Wpf
{
    public static class TextBoxExtensions
    {
        public static T VxTextChanged<T>(this T textBox, Action<string> onChanged) where T : TextBox
        {
            textBox.TextChanged += delegate
            {
                onChanged(textBox.Text);
            };

            return textBox;
        }
    }
}

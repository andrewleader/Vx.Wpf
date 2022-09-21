using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vx.Wpf.TestApp.Components.Pages
{
    internal class TextBoxExampleComponent : VxComponent
    {
        private readonly VxState<string> _name = new VxState<string>("");

        protected override VxElement Render()
        {
            return new VxStackPanel
            {
                Margin = new Thickness(24),
                Children =
                {
                    new VxTextBlock
                    {
                        Text = "Your name",
                    },

                    new VxTextBox
                    {
                        Text = _name.Value,
                        TextChanged = t => _name.Value = t.Text
                    },

                    new VxTextBlock
                    {
                        Text = $"Hello {_name.Value}!",
                        Margin = new Thickness(0, 12, 0, 0)
                    }
                }
            };
        }
    }
}

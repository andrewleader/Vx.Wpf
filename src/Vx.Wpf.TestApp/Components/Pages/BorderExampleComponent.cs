using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Vx.Wpf.TestApp.Components.Pages
{
    internal class BorderExampleComponent : VxComponent
    {
        private readonly VxState<bool> _isTextBox = new VxState<bool>(false);

        protected override VxElement Render()
        {
            return new VxStackPanel
            {
                Margin = new System.Windows.Thickness(24),
                Children =
                {
                    new VxTextBlock
                    {
                        Text = "Border with child that changes type..."
                    },

                    new VxBorder
                    {
                        Child = _isTextBox ? new VxTextBox() : new VxTextBlock
                        {
                            Text = "TextBlock"
                        },
                        Background = new SolidColorBrush(Color.FromArgb(25, 255, 0, 0)),
                        Padding = new System.Windows.Thickness(12)
                    },

                    new VxTextBlock
                    {
                        Text = "Border with child that changes properties"
                    },

                    new VxBorder
                    {
                        Child = new VxTextBlock
                        {
                            Text = "IsTextBox: " + _isTextBox.Value
                        },
                        Background = new SolidColorBrush(Color.FromArgb(25, 255, 0, 0)),
                        Padding = new System.Windows.Thickness(12)
                    },

                    new VxTextBlock
                    {
                        Text = "Border with stack panel that has nested child that changes properties"
                    },

                    new VxBorder
                    {
                        Child = new VxStackPanel
                        {
                            Children =
                            {
                                new VxTextBlock
                                {
                                    Text = "Line 1"
                                },

                                new VxTextBlock
                                {
                                    Text = "IsTextBox: " + _isTextBox.Value
                                }
                            }
                        },
                        Background = new SolidColorBrush(Color.FromArgb(25, 255, 0, 0)),
                        Padding = new System.Windows.Thickness(12)
                    },

                    new VxButton
                    {
                        Content = "Change",
                        Click = b => _isTextBox.Value  = !_isTextBox.Value
                    }
                }
            };
        }
    }
}

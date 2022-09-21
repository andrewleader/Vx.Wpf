using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Vx.Wpf.TestApp.Components.Pages
{
    internal class StackPanelExampleComponent : VxComponent
    {
        private readonly SolidColorBrush _backgroundColor = new SolidColorBrush(Color.FromArgb(25, 255, 0, 0));

        protected override VxElement Render()
        {
            return new VxStackPanel
            {
                Margin = new Thickness(24),
                Background = _backgroundColor,
                Children =
                {
                    new VxTextBlock
                    {
                        Text = "Parent vertical stack panel, with a horizontal stack panel below this text...",
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 12)
                    },

                    new VxStackPanel
                    {
                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                        Background = _backgroundColor,
                        Children =
                        {
                            new VxTextBlock
                            {
                                Text = "Horizontal txt 1",
                                Margin = new Thickness(6)
                            },

                            new VxTextBlock
                            {
                                Text = "Horizontal txt 2",
                                Margin = new Thickness(6)
                            }
                        }
                    }
                }
            };
        }
    }
}

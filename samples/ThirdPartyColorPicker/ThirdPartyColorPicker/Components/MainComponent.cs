using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vx.Wpf;

namespace ThirdPartyColorPicker.Components
{
    internal class MainComponent : VxComponent
    {
        private readonly VxState<Color> _color = new VxState<Color>(Colors.Red);

        protected override VxElement Render()
        {
            return new VxStackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new VxStandardColorPicker
                    {
                        SelectedColor = _color.Value,
                        ColorChanged = p => _color.Value = p.SelectedColor,
                        Width = 300
                    },

                    new VxStackPanel
                    {
                        Width = 300,
                        Margin = new Thickness(24),
                        Children =
                        {
                            new VxTextBlock
                            {
                                Text = "Selected color"
                            },

                            new VxBorder
                            {
                                Background = new SolidColorBrush(_color.Value),
                                Height = 280
                            }
                        }
                    }
                }
            };
        }
    }
}

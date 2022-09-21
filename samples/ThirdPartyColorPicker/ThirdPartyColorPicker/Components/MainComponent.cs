using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vx.Wpf;
using Vx.Wpf.Core.ElementExtensions;

namespace ThirdPartyColorPicker.Components
{
    internal class MainComponent : VxComponent
    {
        private readonly VxState<Color> _color = new VxState<Color>(Colors.Red);

        protected override VxElement Render()
        {
            return new VxGrid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(300) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Children =
                {
                    RenderColumn("Third-party control...", new VxStandardColorPicker
                    {
                        SelectedColor = _color.Value,
                        ColorChanged = p => _color.Value = p.SelectedColor
                    }),

                    RenderColumn("Selected color (our own controls)...", new VxBorder
                    {
                        Background = new SolidColorBrush(_color.Value)
                    }).AttachProperties(el => Grid.SetColumn(el, 1))
                }
            };
        }

        private VxElement RenderColumn(string header, VxElement content)
        {
            return new VxGrid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                Margin = new Thickness(24),
                Children =
                {
                    new VxTextBlock
                    {
                        Text = header
                    },

                    content.AttachProperties(el => Grid.SetRow(el, 1))
                }
            };
        }
    }
}

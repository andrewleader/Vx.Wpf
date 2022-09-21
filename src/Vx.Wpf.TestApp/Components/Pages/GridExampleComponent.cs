using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Vx.Wpf.Core.ElementExtensions;

namespace Vx.Wpf.TestApp.Components.Pages
{
    internal class GridExampleComponent : VxComponent
    {
        protected override VxElement Render()
        {
            return new VxGrid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star),
                    },

                    new ColumnDefinition
                    {
                        Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star)
                    }
                },
                Children =
                {
                    new VxBorder
                    {
                        Background = new SolidColorBrush(Colors.Blue),
                        Child = new VxTextBlock
                        {
                            Text = "First column, Weight = 1",
                            Foreground = new SolidColorBrush(Colors.White)
                        }
                    }.AttachProperties(el => Grid.SetColumn(el, 0)),

                    new VxBorder
                    {
                        Background = new SolidColorBrush(Colors.Red),
                        Child = new VxTextBlock
                        {
                            Text = "Second column, Weight = 2",
                            Foreground = new SolidColorBrush(Colors.White)
                        }
                    }.AttachProperties(el => Grid.SetColumn(el, 1))
                }
            };
        }
    }
}

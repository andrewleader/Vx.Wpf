using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vx.Wpf;

namespace BasicSample.Components
{
    internal class MainComponent : VxComponent
    {
        private VxState<int> _count = new VxState<int>(0);

        protected override VxElement Render()
        {
            return new VxStackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Children =
                {
                    new VxTextBlock
                    {
                        Text = "Vx.Wpf",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center
                    },

                    new VxTextBlock
                    {
                        Text = "Write your WPF UI in a declarative manner!",
                        TextAlignment = TextAlignment.Center
                    },

                    new VxButton
                    {
                        Content = _count.Value == 0 ? "Click me" : "Clicked!",
                        Click = b => _count.Value++,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 12, 0, 0),
                        Padding = new Thickness(4)
                    },

                    new VxTextBlock
                    {
                        Text = $"Clicked {_count.Value} time{(_count.Value == 1 ? "" : "s")}",
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 3, 0, 0)
                    }
                }
            };
        }
    }
}

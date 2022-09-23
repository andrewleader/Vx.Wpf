using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vx.Wpf.TestApp.Components.Pages
{
    internal class ScrollViewerExampleComponent : VxComponent
    {
        protected override VxElement Render()
        {
            var sp = new VxStackPanel();

            for (int i = 1; i < 50; i++)
            {
                sp.Children.Add(new VxTextBlock { Text = "Line " + i });
            }

            return new VxScrollViewer
            {
                Height = 300,
                Content = sp
            };
        }
    }
}

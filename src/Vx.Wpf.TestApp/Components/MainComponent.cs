using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vx.Wpf.TestApp.Components.Pages;

namespace Vx.Wpf.TestApp.Components
{
    internal class MainComponent : VxComponent
    {
        private readonly Type[] _pages = new Type[]
        {
            typeof(TextBoxExampleComponent),
            typeof(StackPanelExampleComponent),
            typeof(BorderExampleComponent)
        };

        private readonly VxState<Type?> _selectedPage = new VxState<Type?>(null);

        protected override VxElement Render()
        {
            if (_selectedPage.Value == null)
            {
                return RenderHome();
            }
            else
            {
                return RenderPage();
            }
        }

        private VxElement RenderHome()
        {
            var sp = new VxStackPanel
            {
                Margin = new System.Windows.Thickness(24),
            };

            foreach (var p in _pages)
            {
                sp.Children.Add(new VxButton
                {
                    Content = p.Name,
                    Click = b => _selectedPage.Value = p
                });
            }

            return sp;
        }

        private VxElement RenderPage()
        {
            return new VxStackPanel
            {
                Children =
                {
                    new VxButton
                    {
                        Content = "Go back",
                        Click = b => _selectedPage.Value = null
                    },

                    Activator.CreateInstance(_selectedPage.Value!) as VxComponent
                }
            };
        }
    }
}

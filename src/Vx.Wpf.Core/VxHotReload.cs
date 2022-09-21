using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vx.Wpf
{
    internal static class VxHotReload
    {
        public static event EventHandler ReRender;

        public static void ClearCache(Type[]? types)
        {

        }

        public static void UpdateApplication(Type[]? types)
        {
            ReRender?.Invoke(null, null);
        }
    }
}

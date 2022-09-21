using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vx.Wpf.SourceGenerator
{
    internal class VxProperty
    {
        public string Name { get; set; }

        public ITypeSymbol PropertyType { get; set; }

        public string StringType { get; set; }

        public bool CanWrite { get; set; }

        public string DefaultValue { get; set; }
    }
}

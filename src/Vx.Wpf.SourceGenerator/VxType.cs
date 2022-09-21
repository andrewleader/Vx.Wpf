using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vx.Wpf.SourceGenerator
{
    internal class VxType
    {
        public string Name { get; set; }

        public VxProperty[] Properties { get; set; }

        public ITypeSymbol UIType { get; set; }
    }
}

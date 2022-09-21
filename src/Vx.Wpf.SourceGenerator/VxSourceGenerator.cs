using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vx.Wpf.SourceGenerator;

namespace Vx.Wpf
{
    [Generator]
    internal class VxSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            foreach (var vxType in GenerateVxTypes(context).ToArray())
            {
                switch (vxType.Name)
                {
                    // These classes have a couple issues I haven't root caused
                    case "VxDataGridHeaderBorder":
                    case "VxRibbonWindow":
                    case "VxBulletChrome":
                    case "VxRibbon":
                    case "VxNavigationWindow":
                    case "VxRibbonGallery":
                    case "VxRibbonRadioButton":
                    case "VxRibbonCheckBox":
                    case "VxRibbonToggleButton":
                    case "VxRibbonToolTip":
                    case "VxButtonChrome":
                    case "VxListBoxChrome":
                    case "VxScrollChrome":
                    case "VxSystemDropShadowChrome":
                        continue;
                }

                string src = TypesToString.Translate(vxType);

                //if (vxType.Name == "VxImage")
                //{
                //    throw new NotImplementedException(src.Replace("\n", " "));
                //}

                context.AddSource(vxType.Name + ".g.cs", src);
            }
        }

        public IEnumerable<VxType> GenerateVxTypes(GeneratorExecutionContext context)
        {
            var seen = new HashSet<string>();

            foreach (var type in GetAllControls(context))
            {
                if (!seen.Add(type.Name))
                {
                    continue;
                }

                yield return GenerateType(type);
            }
        }

        private static readonly string[] _propertyTypesToIgnore = new string[]
        {
            "System.Windows.Media.Transform", // From RenderTransform
            "System.Windows.Point", // From RenderTransformOrigin
            "System.Windows.Media.Media3D.Transform3D",
            "System.Windows.DataTemplate",
            "System.Windows.Controls.ControlTemplate",
            "System.Windows.Style",
            "System.Windows.ResourceDictionary"
        };

        private static IEnumerable<IPropertySymbol> GetProperties(ITypeSymbol type)
        {
            foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
            {
                if (!prop.IsStatic && prop.DeclaredAccessibility == Accessibility.Public && prop.GetMethod != null)
                {
                    yield return prop;
                }
            }

            if (type.BaseType != null)
            {
                foreach (var baseProp in GetProperties(type.BaseType))
                {
                    yield return baseProp;
                }
            }
        }

        private static IEnumerable<IEventSymbol> GetEvents(ITypeSymbol type)
        {
            foreach (var e in type.GetMembers().OfType<IEventSymbol>())
            {
                if (!e.IsStatic && e.DeclaredAccessibility == Accessibility.Public)
                {
                    yield return e;
                }
            }

            if (type.BaseType != null)
            {
                foreach (var baseE in GetEvents(type.BaseType))
                {
                    yield return baseE;
                }
            }
        }

        private static VxType GenerateType(ITypeSymbol type)
        {
            try
            {
                string fullTypeName = type.FullName();

                var props = new List<VxProperty>();

                HashSet<string> seenProps = new HashSet<string>();

                foreach (var prop in GetProperties(type))
                {
                    // We already handled the property (the upper class declared the property using "new" modifier, don't want to assign lower-level ones)
                    if (!seenProps.Add(prop.Name))
                    {
                        continue;
                    }

                    var propType = prop.Type;

                    if (_propertyTypesToIgnore.Contains(propType.FullName()))
                    {
                        continue;
                    }

                    if (prop.IsReadOnly)
                    {
                        // Children and other similar properties
                        //if (_uiElementCollectionType.IsAssignableFrom(propType))
                        if (prop.Name == "Children")
                        {
                            props.Add(new VxProperty
                            {
                                Name = prop.Name,
                                PropertyType = propType,
                                CanWrite = false
                            });
                        }
                    }
                    else
                    {
                        // TODO: Basic filtering of the below
                        switch (prop.Name)
                        {
                            case "SelectionLength":
                            case "SelectionStart":
                            case "SelectedItem":
                            case "PreferredPasteFormats":
                                continue;
                        }

                        // Don't assign text-related fields that are based on current editing values
                        //if (prop.DeclaringType == _textBoxType)
                        //{
                        //    switch (prop.Name)
                        //    {
                        //        case nameof(TextBox.SelectionLength):
                        //        case nameof(TextBox.SelectionStart):
                        //            continue;
                        //    }
                        //}

                        //// Don't assign ListView SelectedItem
                        //if (prop.DeclaringType == _listViewType)
                        //{
                        //    switch (prop.Name)
                        //    {
                        //        case nameof(ListView.SelectedItem):
                        //            continue;
                        //    }
                        //}

                        //// Don't support InkCanvas PreferredPasteFormats yet, it requires IEnumerable
                        //if (prop.DeclaringType == _inkCanvasType)
                        //{
                        //    switch (prop.Name)
                        //    {
                        //        case nameof(InkCanvas.PreferredPasteFormats):
                        //            continue;
                        //    }
                        //}

                        props.Add(new VxProperty
                        {
                            Name = prop.Name,
                            PropertyType = propType,
                            CanWrite = true
                        });
                    }
                }

                //if (_buttonBaseType.IsAssignableFrom(type))
                //if (type.Name == "Button")
                //{
                //    // Add the Click property
                //    props.Add(new VxProperty
                //    {
                //        Name = "Click",
                //        DynamicType = typeof(Action),
                //        CanWrite = true
                //    });
                //}

                //if (_textBoxType.IsAssignableFrom(type))
                //if (type.Name == "TextBox")
                //{
                //    // Add the TextChanged property
                //    props.Add(new VxProperty
                //    {
                //        Name = "TextChanged",
                //        DynamicType = typeof(Action<string>),
                //        CanWrite = true
                //    });
                //}

                foreach (var e in GetEvents(type))
                {
                    props.Add(new VxProperty
                    {
                        Name = e.Name,
                        StringType = "System.Action<" + fullTypeName + ">",
                        CanWrite = true
                    });
                }

                return new VxType
                {
                    Name = "Vx" + type.Name,
                    Properties = props.ToArray(),
                    UIType = type
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed in GenerateType with type " + type.Name + ". " + ex.ToString().Replace("\n", " "));
            }
        }

        /// <summary>
        /// THIS WORKS!!
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<INamedTypeSymbol> GetSystemWindowsControls(GeneratorExecutionContext context)
        {
            var stackPanelType = context.Compilation.GetTypeByMetadataName("System.Windows.Controls.StackPanel");
            var typesInControls = stackPanelType.ContainingNamespace.GetTypeMembers();
            var uiTypesInControls = typesInControls.Where(i => ShouldSelect(i));
            return uiTypesInControls;
        }

        public IEnumerable<ITypeSymbol> GetAllControls(GeneratorExecutionContext context)
        {
            return GetAllControls(context.Compilation.GlobalNamespace);
        }

        public IEnumerable<ITypeSymbol> GetAllControls(INamespaceSymbol ns)
        {
            foreach (var type in ns.GetTypeMembers())
            {
                if (ShouldSelect(type))
                {
                    yield return type;
                }
            }

            foreach (var childNS in ns.GetNamespaceMembers())
            {
                foreach (var answer in GetAllControls(childNS))
                {
                    yield return answer;
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // Syntax notifications are only for the code being compiled, doesn't include code from libraries
            //context.RegisterForSyntaxNotifications(() => new UIElementClassFinder());
        }

        private static bool ShouldSelect(INamedTypeSymbol type)
        {
            try
            {
                return type.IsType
                    && type.DeclaredAccessibility == Accessibility.Public
                    && !type.IsGenericType
                    && type.InstanceConstructors.Any(i => i.DeclaredAccessibility == Accessibility.Public && i.Parameters.IsEmpty)
                    && IsUIElementType(type);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed in ShouldSelect: " + ex);
            }
        }

        private static bool IsUIElementType(INamedTypeSymbol type)
        {
            try
            {
                if (type.BaseType != null)
                {
                    if (type.BaseType.Name == "UIElement" && type.FullNamespace() == "System.Windows")
                    {
                        return true;
                    }
                    else
                    {
                        return IsUIElementType(type.BaseType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed in IsUIElementType: " + ex);
            }

            return false;
        }

        private static IList<Type> GetInstances(Assembly assembly, Type type)
        {
            var rootType = type;

            return (from t in assembly.GetTypes()
                    where t.IsSubclassOf(rootType) && t.IsPublic && t.GetConstructors().Length > 0 && !t.IsGenericType
                    select t).ToList();
        }
    }

    internal static class Extensions
    {
        public static string FullName(this ITypeSymbol type)
        {
            if (type.ContainingNamespace == null)
            {
                return type.Name;
            }

            return type.FullNamespace() + "." + type.Name;
        }

        public static string FullNamespace(this ITypeSymbol type)
        {
            return type.ContainingNamespace.FullName();
        }

        public static string FullName(this INamespaceSymbol ns)
        {
            if (ns.ContainingNamespace == null || ns.ContainingNamespace.IsGlobalNamespace)
            {
                return ns.Name;
            }

            return ns.ContainingNamespace.FullName() + "." + ns.Name;
        }
    }
}

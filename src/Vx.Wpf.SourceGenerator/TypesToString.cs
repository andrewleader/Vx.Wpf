using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vx.Wpf.SourceGenerator
{
    internal static class TypesToString
    {
        public static string Translate(VxType[] types)
        {
            StringBuilder answer = new StringBuilder();

            answer.Append("namespace Vx.Wpf\n{\n");

            foreach (var type in types)
            {
                Translate(type, answer);

                answer.Append("\n\n");
            }

            answer.Append("}");

            return answer.ToString();
        }

        public static string Translate(VxType type)
        {
            StringBuilder answer = new StringBuilder();

            answer.Append("namespace Vx.Wpf\n{\n");

            Translate(type, answer);

            answer.Append("\n\n");

            answer.Append("}");

            return answer.ToString();
        }

        private static string WritePropertyType(ITypeSymbol propertyType)
        {
            if (propertyType.NullableAnnotation == NullableAnnotation.Annotated)
            {
                return propertyType.ToDisplayString();
            }

            return propertyType.ToDisplayString();
        }

        private static string WriteDynamicType(Type propertyType)
        {
            var nullableType = Nullable.GetUnderlyingType(propertyType);
            if (nullableType != null)
            {
                return WriteDynamicType(nullableType) + "?";
            }

            return propertyType.FullName;
        }

            private static void Translate(VxType type, StringBuilder builder)
        {
            builder.Append($"public class {type.Name} : VxElement\n{{\n");

            builder.Append($"protected override System.Type UIType => typeof({type.UIType.FullName()});\n\n");

            foreach (var prop in type.Properties)
            {
                //if (_uiElementCollectionType.IsAssignableFrom(prop.PropertyType))
                if (prop.Name == "Children") // TODO: Should verify it's a children list, but good enough for now
                {
                    builder.Append($"public System.Collections.Generic.List<VxElement> {prop.Name} {{ get; }} = new System.Collections.Generic.List<VxElement>();");
                }
                else if (prop.StringType != null)
                {
                    builder.Append($"public {prop.StringType} {prop.Name} {{ get; ");

                    if (prop.CanWrite)
                    {
                        builder.Append("set; ");
                    }

                    builder.Append("}");
                }
                else
                {
                    builder.Append($"public {WritePropertyType(prop.PropertyType)} {prop.Name} {{ get; ");

                    if (prop.CanWrite)
                    {
                        builder.Append("set; ");
                    }

                    builder.Append("}");
                }

                if (prop.DefaultValue != null)
                {
                    builder.Append(" = ");
                    builder.Append(prop.DefaultValue);
                    builder.Append(";");
                }

                builder.Append("\n");
            }

            builder.Append("}");
        }
    }
}

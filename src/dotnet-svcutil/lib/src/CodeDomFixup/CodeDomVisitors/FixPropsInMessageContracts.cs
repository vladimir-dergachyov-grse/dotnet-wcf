// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;
using Microsoft.Xml.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class FixPropsInMessageContracts : MessageContractVisitor
    {
        public FixPropsInMessageContracts()
        {
        }
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            var membersCount = type.Members.Count;
            for (int i = 0; i < membersCount; ++i)
            {
                var prop = type.Members[i] as CodeMemberProperty;
                if (prop == null)
                {
                    continue;
                }

                bool defaultValue = false;
                bool optional = false;
                bool valueType = false;
                bool nillable = false;

                foreach (CodeAttributeDeclaration declaration in prop.CustomAttributes)
                {
                    switch (declaration.Name)
                    {
                        case "Onvif.Property.InitExpression":
                            defaultValue = true;
                            break;
                        case "Onvif.Property.Optional":
                            optional = true;
                            break;
                        case "Onvif.Property.ValueType":
                            valueType = true;
                            break;
                        case "Onvif.Property.Nillable":
                            nillable = true;
                            break;
                    }
                }

                prop.Nullable = !defaultValue && (nillable || optional);
                prop.Required = !(defaultValue || nillable || optional);

                if (valueType && !defaultValue && !nillable && optional)
                {
                    var propSerialized = new CodeMemberProperty
                    {
                        Type = new CodeTypeReference(typeof(string)),
                        Name = $"_{prop.Name}Serialized",
                        HasGet = true,
                        HasSet = true,
                        CustomAttributes = prop.CustomAttributes,
                        Nullable = true
                    };

                    prop.CustomAttributes = new CodeAttributeDeclarationCollection
                    {
                        new CodeAttributeDeclaration(typeof(XmlIgnoreAttribute).FullName)
                    };

                    foreach (CodeAttributeDeclaration attr in propSerialized.CustomAttributes)
                    {
                        if (attr.Name == typeof(System.ServiceModel.MessageBodyMemberAttribute).FullName)
                        {
                            attr.Arguments.Insert(0, new CodeAttributeArgument("Name", new CodePrimitiveExpression(prop.Name)));
                        }
                    }

                    var propTypeParts = prop.Type.BaseType.Split('.');
                    if (propTypeParts.Length > 0)
                    {
                        var propType = propTypeParts[propTypeParts.Length - 1];

                        propSerialized.GetString = $"=> {prop.Name}.HasValue ? System.Xml.XmlConvert.ToString({prop.Name}.Value) : null;";
                        propSerialized.SetString = $"=> {prop.Name} = string.IsNullOrEmpty(value) ? null : System.Xml.XmlConvert.To{propType}(value);";

                        type.Members.Add(propSerialized);
                    }
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.CodeDom;
using Microsoft.Xml.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class FixPropsInMessageContracts : MessageContractVisitor
    {
        private readonly IReadOnlyDictionary<string, string> _table = new Dictionary<string, string>()
        {
            ["System.Boolean"] = "ToBoolean",
            ["System.Char"] = "ToChar",
            ["System.SByte"] = "ToSByte",
            ["System.Byte"] = "ToByte",
            ["System.Int16"] = "ToInt16",
            ["System.UInt16"] = "ToUInt16",
            ["System.Int32"] = "ToInt32",
            ["System.UInt32"] = "ToUInt32",
            ["System.Int64"] = "ToInt64",
            ["System.UInt64"] = "ToUInt64",
            ["System.Single"] = "ToSingle",
            ["System.Double"] = "ToDouble",
            ["System.Decimal"] = "ToDecimal",
            ["System.TimeSpan"] = "ToTimeSpan",
            ["System.DateTime"] = "ToDateTime",
            ["System.DateTimeOffset"] = "ToDateTimeOffset",
            ["System.Guid"] = "ToGuid",
        };

        public FixPropsInMessageContracts()
        {
        }
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            bool fixProp(CodeMemberProperty prop)
            {
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

                    var propTypeParts = prop.Type.BaseType.Split('.');
                    if (propTypeParts.Length > 0)
                    {
                        var propType = propTypeParts[propTypeParts.Length - 1];

                        propSerialized.GetString = $"=> {prop.Name}.HasValue ? System.Xml.XmlConvert.ToString({prop.Name}.Value) : null;";
                        propSerialized.SetString = $"=> {prop.Name} = string.IsNullOrEmpty(value) ? null : System.Xml.XmlConvert.To{propType}(value);";

                        type.Members.Add(propSerialized);
                    }
                }

                return true;
            }

            CollectionHelpers.MapList<CodeMemberProperty>(type.Members, fixProp, null);
        }
    }
}

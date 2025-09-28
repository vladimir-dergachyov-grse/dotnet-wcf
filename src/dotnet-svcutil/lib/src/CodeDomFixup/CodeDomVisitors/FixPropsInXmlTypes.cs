// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;
using Microsoft.Xml.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class FixPropsInXmlTypes : XmlTypeVisitor
    {
        public FixPropsInXmlTypes()
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
                    var propSpecified = new CodeMemberProperty()
                    {
                        Type = new CodeTypeReference(typeof(bool)),
                        Name = $"{prop.Name}Specified",
                        HasGet = true,
                        HasSet = true
                    };

                    propSpecified.Attributes = (propSpecified.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
                    propSpecified.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(XmlIgnoreAttribute).FullName));

                    propSpecified.GetString = $"=> {prop.Name}.HasValue;";
                    propSpecified.SetString = $"=> {prop.Name} = value ? {prop.Name} : null;";

                    type.Members.Add(propSpecified);
                }

                return true;
            }

            CollectionHelpers.MapList<CodeMemberProperty>(type.Members, fixProp, null);
        }
    }
}

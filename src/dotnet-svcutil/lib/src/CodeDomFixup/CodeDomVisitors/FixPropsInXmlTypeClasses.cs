// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;
using Microsoft.Xml.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class FixPropsInXmlTypeClasses : XmlTypeVisitor
    {
        public FixPropsInXmlTypeClasses()
        {
        }
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            if (!type.IsClass)
            {
                return;
            }

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
            }
        }
    }
}

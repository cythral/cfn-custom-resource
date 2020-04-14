using System;
using System.Diagnostics;
using Cythral.CodeGeneration.Roslyn;

namespace Cythral.CloudFormation.CustomResource.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    [CodeGenerationAttribute("Cythral.CloudFormation.CustomResource.Generator.Generator, Cythral.CloudFormation.CustomResource.Generator")]
    public class CustomResourceAttribute : Attribute
    {
        public CustomResourceAttribute() { }

        public Type ResourcePropertiesType { get; set; }

        public object[] Grantees { get; set; }

        public GranteeType GranteeType { get; set; }
    }
}
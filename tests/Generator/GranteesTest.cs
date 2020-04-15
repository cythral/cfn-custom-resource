using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Cythral.CodeGeneration.Roslyn;

using Cythral.CloudFormation.CustomResource.Generator;
using Cythral.CloudFormation.CustomResource.Attributes;
using static Cythral.CloudFormation.CustomResource.Attributes.GranteeType;

using NUnit.Framework;

using RichardSzalay.MockHttp;

namespace Tests
{
    // just test that this compiles correctly
    [CustomResource(
        Grantees = new string[] { "cfn-metadata:DevAgentRoleArn", "cfn-metadata:ProdAgentRoleArn" },
        GranteeType = Import,
        ResourcePropertiesType = typeof(CustomResourceWithGrantees.Properties)
    )]
    public partial class CustomResourceWithGrantees : TestCustomResource
    {
        public class Properties
        {

        }
    }
}
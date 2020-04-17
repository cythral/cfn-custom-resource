using System;
using System.Linq;
using System.Threading.Tasks;

using Cythral.CloudFormation.CustomResource.Attributes;
using Cythral.CloudFormation.CustomResource.Core;

using NUnit.Framework;
using FluentAssertions;

namespace Tests
{
    public partial class ResourcePropertiesTypeTests
    {
        [CustomResource]
        public partial class DefaultedPropertiesTypeResource
        {
            public class Properties
            {

            }

            public Task<Response> Create()
            {
                return Task.FromResult(new Response());
            }

            public Task<Response> Update()
            {
                return Task.FromResult(new Response());
            }

            public Task<Response> Delete()
            {
                return Task.FromResult(new Response());
            }
        }

        [Test]
        public void DefaultingWorks()
        {
            Type requestType = (
                from member in typeof(DefaultedPropertiesTypeResource).GetProperties()
                where member.Name == "Request"
                select member.PropertyType
            ).First();

            Type resourcePropertiesType = (
                from member in requestType.GetProperties()
                where member.Name == "ResourceProperties"
                select member.PropertyType
            ).First();

            resourcePropertiesType.Should().Be(typeof(DefaultedPropertiesTypeResource.Properties));
        }
    }
}
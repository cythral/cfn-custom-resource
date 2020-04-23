# Cythral.CloudFormation.CustomResource

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/cythral/cfn-custom-resource/Continuous%20Integration?style=flat-square) ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Cythral.CloudFormation.CustomResource?color=blue&style=flat-square)

Easily create custom resources for AWS in C# using Cythral.CloudFormation.CustomResource, that are:

- **CI Friendly** - no need to workaround CLIs that deploy things for you. We generate a CloudFormation template, you can choose how you want to package and deploy it. (We provide msbuild tasks for this too.)
- **Code-first** - no need to mess with JSON schemas. Write your property specification using aspect-oriented programming techniques in C# via Attributes.
- **Easy to read** - just write the property specification along with create, update and delete methods. The rest is done behind the scenes!
- **Secure** - if you use the AWS SDK to make API calls, we'll collect the minimum IAM permissions you need and add it to the generated CloudFormation template.

## Table of Contents

1. [Installation](#installation)
2. [Usage](#usage)
   1. [Custom Resource Development](#custom-resource-development)
   2. [Template Customization](#template-customization)
   3. [Template Packaging and Deployment](#template-packaging-and-deployment)
3. [Acknowledgements](#acknowledgements)
4. [License](#license)

## Installation

Install using the dotnet CLI:

```shell
dotnet add package Cythral.CloudFormation.CustomResource --version 0.2.22-alpha-gae48b9755e
```

Or add directly to your project file:

```xml
<ItemGroup>
    <PackageReference Include="Cythral.CloudFormation.CustomResource" Version="0.2.22-alpha-gae48b9755e" />
</ItemGroup>
```

## Usage

### Custom Resource Development

```cs
using System;
using System.ComponentModel.DataAnnotations;

using Cythral.CloudFormation.CustomResource.Attributes;
using static Cythral.CloudFormation.CustomResource.Attributes.GranteeType;
using Cythral.CloudFormation.CustomResource.Core;

namespace Example
{
    [CustomResource(
        // Optional. Sets the type to deserialize ResourceProperties to.
        // If not set, this will default to the 'Properties' subclass
        ResourcePropertiesType = typeof(ExampleCustomResource.Properties),
        // Required if Grantees is set. Sets the grantee type to use
        // Possible values are Import or Literal
        GranteeType = Import
        // Optional. Creates a lambda permission and grants invoke access to the
        // specified roles.  The roles can be imported (via !ImportValue) or hardcoded
        Grantees = new string[] { "ImportedValue" },
    )]
    public partial class ExampleCustomResource
    {
        // ** The class MUST be marked with the partial modifier **

        public class Properties
        {
            // Data annotation attributes will be evaluated.  Ex. If you add [Required]
            // and the request did not contain a Name, the CustomResource will return a failed response
            [Required]
            public string Name { get; set; }

            // If a change is detected for this property, your create method will be called instead of update.
            // Assuming you return a new PhysicalResourceId, CloudFormation will invoke the CustomResource again
            // with a DELETE request for the old PhysicalResourceId.
            [UpdateRequiresReplacement]
            public string Version { get; set; }
        }

        // This will get invoked on create requests or when update
        public async Task<Response> Create()
        {
            // requires replacement.

            return Task.FromResult(new Response
            {
                PhysicalResourceId = "123", // ID of the created resource
                Data = new {
                    // Extra data to return back to CloudFormation
                }
            });
        }

        // This will get invoked on update requests.
        public async Task<Response> Update()
        {
            // If you throw an exception, the message will get passed
            // back to CloudFormation as the reason for failure.
            throw new Exception("Error message");
        }

        // this will get invoked on delete requests.
        public async Task<Response> Delete()
        {
        }
    }
}
```

### Template Customization

The library will generate a CloudFormation template and write it to `$(OutDir)/$(AssemblyName).template.yml`. Apart from grantees customization, you can also customize the stack description. Because you can have multiple custom resources in the same stack, this is done by adding a `StackDescription` property to your csproj file:

```xml
<PropertyGroup>
    <StackDescription>Description of your custom resource stack here.</StackDescription>
</PropertyGroup>
```

### Template Packaging and Deployment

To package and deploy the generated template to AWS, you will need two things:

1. Install the AWS CLI
2. A DeploymentBucket property to your csproj file that is set to the name of the S3 bucket you want artifacts uploaded to. This defaults to your AssemblyName.

Then, run one of the following commands:

```shell
dotnet publish
```

If you want to package and upload artifacts to S3, but not deploy the project, run this command:

```shell
dotnet publish -p:Deploy=false
```

If you want to simply run the publish target without packaging or deploying, run this command:

```shell
dotnet publish -p:Package=false -p:Deploy=false
```

## Acknowledgements

Cythral would like to thank Andrew Aarnott (@aarnott), Amadeusz Sadowski (@amis92), Manuel Pfemeter (@manne) and the rest of the contributors of [CodeGeneration.Roslyn](https://github.com/aarnott/codegeneration.roslyn) for their excellent work on that project, which in large part makes this one possible.

## License

This project is licensed under the [MIT License](LICENSE.txt).

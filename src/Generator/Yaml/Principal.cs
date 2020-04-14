using System.Collections.Generic;

namespace Cythral.CloudFormation.CustomResource.Generator.Yaml
{
    public class Principal
    {
        public HashSet<string> AWS;
        public HashSet<string> Service;
    }
}
namespace Cythral.CloudFormation.CustomResource.Generator.Yaml
{
    public class ImportValueTag
    {
        public ImportValueTag(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; set; }
    }
}
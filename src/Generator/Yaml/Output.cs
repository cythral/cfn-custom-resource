namespace Cythral.CloudFormation.CustomResource.Generator.Yaml
{

    public class Output
    {

        public Output(object value, object name)
        {
            Value = value;
            Export = new Export { Name = name };
        }

        public object Value;
        public Export Export;

    }

    public class Export
    {
        public object Name;
    }
}
using System.Net.Http;

namespace Cythral.CloudFormation.CustomResource.Generator
{

    public interface IHttpClientProvider
    {

        HttpClient Provide();

    }

}
using System.Net.Http;

namespace Cythral.CloudFormation.CustomResource.Core
{

    public interface IHttpClientProvider
    {

        HttpClient Provide();

    }

}
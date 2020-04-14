using System.Net.Http;

namespace Cythral.CloudFormation.CustomResource.Generator
{

    public class DefaultHttpClientProvider : IHttpClientProvider
    {

        public HttpClient Provide()
        {
            return new HttpClient();
        }

    }

}
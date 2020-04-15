using System.Net.Http;

namespace Cythral.CloudFormation.CustomResource.Core
{

    public class DefaultHttpClientProvider : IHttpClientProvider
    {

        public HttpClient Provide()
        {
            return new HttpClient();
        }

    }

}
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Clean.Architecture.Template.Api.SwaggerConfigs
{
    public class UnsupportedApiVersion : DefaultErrorResponseProvider
    {
        protected override object CreateErrorContent(ErrorResponseContext context)
        {
            return new
            {
                Message = "Endpoint does not support the specified version"
            };
        }
    }
}

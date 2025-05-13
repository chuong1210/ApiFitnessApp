namespace ApiGatewayFitness.Middleware
{
    public class TokenLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenLoggingMiddleware> _logger;

        public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                _logger.LogInformation("API Gateway - Received Token via Middleware: {Token}", token); // CẨN THẬN VỚI LOGGING TOKEN ĐẦY ĐỦ
            }
            else if (!string.IsNullOrEmpty(authorizationHeader))
            {
                _logger.LogInformation("API Gateway - Received Authorization Header (not Bearer): {Header}", authorizationHeader);
            }

            await _next(context);
        }
    }
    }

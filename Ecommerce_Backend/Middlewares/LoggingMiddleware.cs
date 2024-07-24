namespace Ecommerce_Backend.Middlewares
{
    public class LoggingMiddleware
    {
      
            private readonly RequestDelegate _next;

            public LoggingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    Console.WriteLine("Server time: " + DateTime.UtcNow);
        //    await _next(context);
        //}

        public async Task InvokeAsync(HttpContext context)
        {
            // Log de todos los encabezados de la solicitud
            //Console.WriteLine("Request Headers:");
            //foreach (var header in context.Request.Headers)
            //{
            //    Console.WriteLine($"{header.Key}: {header.Value}");
            //}

            // Log del token recibido
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.Split(" ").Last();
            Console.WriteLine("Token received in Middleware: " + token);

            await _next(context);
        }

    }
}

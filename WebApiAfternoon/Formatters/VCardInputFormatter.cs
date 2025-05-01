using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using WebApiAfternoon.Dtos;
using WebApiAfternoon.Formatters;

namespace WebApiAfternoon.Formatters
{
    public class VCardInputFormatter : TextInputFormatter
    {
        public VCardInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/vcard"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
        {
            var httpContext=context.HttpContext;
            var serviceProvider = httpContext.RequestServices;
            var logger=serviceProvider.GetRequiredService<ILogger<VCardInputFormatter>>();
            using var reader = new StreamReader(httpContext.Request.Body, effectiveEncoding);
            string? nameLine = null;
            try
            {
                await ReadLineAsync("BEGIN:VCARD:", reader, context, logger);
                await ReadLineAsync("VERSION 1.2:", reader, context, logger);

                nameLine=await ReadLineAsync("FN:",reader, context, logger);
                
                var split=nameLine.Split(":");

                var serialLine= await ReadLineAsync("SND:",reader,context, logger);
                var split1=serialLine.Split(":");

                var ageLine = await ReadLineAsync("AGE:", reader, context, logger);
                var split2=ageLine.Split(":");

                var scoreLine = await ReadLineAsync("SCORE:", reader, context, logger);
                var split3=scoreLine.Split(":");

                await ReadLineAsync("END:VCARD:",reader,context,logger);

                var student = new StudentAddDto
                {
                    Fullname = split[1],
                    SeriaNo = split[1],
                    Age = int.Parse(split[1]),
                    Score = int.Parse(split[1]),

                };
                return await InputFormatterResult.SuccessAsync(student);

                

            }
            catch
            {
                logger.LogError("Read failed:nameLine=[nameLine]", nameLine);
                return await InputFormatterResult.FailureAsync();
            }

        }



        private static async Task<string> ReadLineAsync(
            string expectedText, StreamReader reader, InputFormatterContext context,
            ILogger logger)
        {
            var line = await reader.ReadLineAsync();

            if (line is null || !line.StartsWith(expectedText))
            {
                var errorMessage = $"Looked for '{expectedText}' and got '{line}'";

                context.ModelState.TryAddModelError(context.ModelName, errorMessage);
                logger.LogError(errorMessage);

                throw new Exception(errorMessage);
            }

            return line;
        }
    }
}


using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using WebApiAfternoon.Dtos;
using WebApiAfternoon.Entities;


namespace WebApiAfternoon.Formatters
{
    public class VCardOutputFormatter : TextOutputFormatter
    {
        public VCardOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/vcard"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var sb = new StringBuilder();
            if (context.Object is IEnumerable<StudentDto> list)
            {
                foreach (var student in list)
                {

                    FormatVCard(sb, student);
                }
            }
            else if (context.Object is StudentDto item)
            {
                FormatVCard(sb, item);
            }

            await response.WriteAsync(sb.ToString());

        }

        private void FormatVCard(StringBuilder sb, StudentDto student)
        {
            sb.AppendLine("Begin:VCARD");
            sb.AppendLine("VERSION:1.2");
            sb.AppendLine($"FN:{student.Fullname}");
            sb.AppendLine($"SNO:{student.SeriaNo}");
            sb.AppendLine($"AGE:{student.Age}");
            sb.AppendLine($"SCORE:{student.Score}");
            sb.AppendLine($"UDI:{student.Id}");
            sb.AppendLine("END:VCARD");
            sb.AppendLine("");

        }
    }
}


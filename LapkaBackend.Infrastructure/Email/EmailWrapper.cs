using System.Reflection;
using LapkaBackend.Domain.Enums;
using LapkaBackend.Application.Interfaces;
using MimeKit;

namespace LapkaBackend.Infrastructure.Email
{
    public class EmailWrapper : IEmailWrapper
    {
        private readonly string _filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Email", "Account");

        public BodyBuilder GetBuilder(Templates template)
        {
            var builder = new BodyBuilder();

            if (template != Templates.Welcome)
            {
                builder.LinkedResources.Add(Path.Combine(_filePath, "Key.png"));
            }

            builder.LinkedResources.Add(Path.Combine(_filePath, "LappkaLogo.png"));

            builder.HtmlBody = File.ReadAllText(Path.Combine(_filePath, template + ".html"));

            return builder;
        }
    }

    
}

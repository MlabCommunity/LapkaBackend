using LapkaBackend.Application.Enums;
using LapkaBackend.Application.Interfaces;
using MimeKit;
using MimeKit.Utils;
using System.Runtime.CompilerServices;

namespace LapkaBackend.Infrastructure.Email
{
    public class EmailWrapper : IEmailWrapper
    {
        private readonly string _filePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.ToString(), "LapkaBackend.Infrastructure", "Email", "Account");

        public BodyBuilder GetBuilder(Templates template)
        {
            var builder = new BodyBuilder();

            if (template != Templates.Welcome)
            {
                builder.LinkedResources.Add(Path.Combine(_filePath, "Key.png"));
            }

            builder.LinkedResources.Add(Path.Combine(_filePath, "LappkaLogo.png"));

            builder.HtmlBody = File.ReadAllText(Path.Combine(_filePath, template.ToString() + ".html"));

            return builder;
        }
    }

    
}

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nexu.Shared.Infrastructure.Email;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmailSenderRegistrationExtensions
    {
        public static bool AddTemplateEmailSender(this IServiceCollection services, IConfiguration configuration, ILogger logger = null)
        {
            services.AddTransient<ITemplateEngine, RazorTemplateEngine>();
            services.AddTransient<ITemplateEmailSender, TemplateEmailSender>();
            return AddEmailSender(services, configuration, logger);
        }

        public static bool AddEmailSender(this IServiceCollection services, IConfiguration configuration, ILogger logger = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            //if (configuration.GetSection("SES").Exists())
            //{
            //    logger?.LogInformation("Configuring AWS Simple Email Service sender");
            //    services.TryAddSingleton(configuration.GetAWSOptions());
            //    services.TryAddAWSService<IAmazonSimpleEmailService>();
            //    services.AddTransient<IEmailSender, SesEmailSender>();
            //    services.AddOptions<SesOptions>()
            //        .Bind(configuration.GetSection("SES"))
            //        .ValidateDataAnnotations();
            //    return true;
            //}

            if (configuration.GetSection("Mailgun").Exists())
            {
                logger?.LogInformation("Configuring Mailgun email sender");
                services.AddHttpClient<IEmailSender, MailgunEmailSender>();
                services.AddOptions<MailgunOptions>()
                    .Bind(configuration.GetSection("Mailgun"))
                    .ValidateDataAnnotations();
                return true;
            }

            if (configuration.GetSection("Mandrill").Exists())
            {
                logger?.LogInformation("Configuring Mandrill email sender");
                services.AddHttpClient<IEmailSender, MandrillEmailSender>();
                services.AddOptions<MandrillOptions>()
                    .Bind(configuration.GetSection("Mandrill"))
                    .ValidateDataAnnotations();
                return true;
            }

            if (configuration.GetSection("Sendgrid").Exists())
            {
                logger?.LogInformation("Configuring Sendgrid email sender");
                services.AddHttpClient<IEmailSender, SendgridEmailSender>();
                services.AddOptions<SendgridOptions>()
                    .Bind(configuration.GetSection("Sendgrid"))
                    .ValidateDataAnnotations();
                return true;
            }
            if (configuration.GetSection("Smtp").Exists())
            {
                logger?.LogInformation("Configuring Smtp email sender");
                services.AddTransient<IEmailSender, SmtpEmailSender>();
                services.AddOptions<SmtpEmailOptions>()
                    .Bind(configuration.GetSection("Smtp"))
                    .ValidateDataAnnotations();
                return true;
            }

            return false;
        }
    }
}

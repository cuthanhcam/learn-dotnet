# Email Service Configuration

## Overview
DevForge uses an SMTP-based email service for sending transactional emails including:
- Email confirmations
- Password reset requests  
- Welcome emails
- Two-factor authentication notifications

## Configuration

### Development Environment
Add the following configuration to `appsettings.Development.json`:

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@devforge.com",
    "FromName": "DevForge",
    "EnableSsl": true
  }
}
```

### Using Gmail SMTP

1. **Enable 2-Factor Authentication** on your Gmail account
2. **Generate App Password**:
   - Go to Google Account Settings
   - Security → App passwords
   - Select "Mail" and generate password
   - Use the generated 16-character password in `SmtpPassword`

3. **Configuration**:
   ```json
   "Email": {
     "SmtpHost": "smtp.gmail.com",
     "SmtpPort": 587,
     "SmtpUsername": "yourname@gmail.com",
     "SmtpPassword": "generated-app-password",
     "FromEmail": "noreply@yourcompany.com",
     "FromName": "Your Company",
     "EnableSsl": true
   }
   ```

### Using Other SMTP Providers

#### Microsoft 365 / Outlook
```json
"Email": {
  "SmtpHost": "smtp.office365.com",
  "SmtpPort": 587,
  "SmtpUsername": "your-email@outlook.com",
  "SmtpPassword": "your-password",
  "EnableSsl": true
}
```

#### SendGrid
```json
"Email": {
  "SmtpHost": "smtp.sendgrid.net",
  "SmtpPort": 587,
  "SmtpUsername": "apikey",
  "SmtpPassword": "your-sendgrid-api-key",
  "EnableSsl": true
}
```

#### Amazon SES
```json
"Email": {
  "SmtpHost": "email-smtp.us-east-1.amazonaws.com",
  "SmtpPort": 587,
  "SmtpUsername": "your-smtp-username",
  "SmtpPassword": "your-smtp-password",
  "EnableSsl": true
}
```

## Development Mode

If SMTP credentials are not configured (empty username/password), the EmailService will:
- **Log** email attempts instead of sending them
- **Continue** normal application flow without errors
- **Display** email details in application logs

This allows development without configuring real email services.

## Email Templates

The service includes HTML email templates for:

### Email Confirmation
- Professional HTML layout
- Confirmation button
- Fallback link
- Security notice

### Password Reset
- Reset password button
- Expiration warning (1 hour)
- Security disclaimer
- Support information

### Welcome Email
- Friendly greeting
- Feature highlights
- Call to action

## Security Best Practices

1. **Never commit credentials** to source control
2. **Use environment variables** in production:
   ```bash
   export Email__SmtpUsername="your-email"
   export Email__SmtpPassword="your-password"
   ```
3. **Use User Secrets** in development:
   ```bash
   dotnet user-secrets set "Email:SmtpUsername" "your-email"
   dotnet user-secrets set "Email:SmtpPassword" "your-password"
   ```
4. **Enable SSL/TLS** for all SMTP connections
5. **Use app-specific passwords** instead of account passwords

## Testing Emails

### Local Testing with Papercut
[Papercut](https://github.com/ChangemakerStudios/Papercut-SMTP) is a free SMTP test server:

```json
"Email": {
  "SmtpHost": "localhost",
  "SmtpPort": 25,
  "SmtpUsername": "",
  "SmtpPassword": "",
  "EnableSsl": false
}
```

### Testing with MailHog
[MailHog](https://github.com/mailhog/MailHog) captures emails in development:

```json
"Email": {
  "SmtpHost": "localhost",
  "SmtpPort": 1025,
  "SmtpUsername": "",
  "SmtpPassword": "",
  "EnableSsl": false
}
```

## Monitoring

The EmailService logs all email operations:
- **Information**: Successful email sends
- **Warning**: SMTP not configured (development mode)
- **Error**: Failed email delivery with exception details

Check application logs for email-related activity:
```
Email confirmation sent to user@example.com for user johndoe
Password reset email sent to user@example.com for user johndoe
```

## Troubleshooting

### Common Issues

**Authentication Failed**
- Verify username and password are correct
- Check if 2FA requires app password
- Ensure account is not locked

**Connection Refused**
- Verify SMTP host and port
- Check firewall settings
- Ensure EnableSsl matches provider requirements

**Email Not Received**
- Check spam/junk folder
- Verify recipient email is valid
- Check SMTP provider sending limits
- Review application logs for errors

### Debug Mode
Enable detailed SMTP logging in `appsettings.Development.json`:
```json
"Logging": {
  "LogLevel": {
    "DevForge.Infrastructure.Services.EmailService": "Debug"
  }
}
```

## Future Enhancements

Potential improvements for production:
- Queue-based email delivery (Hangfire/MassTransit)
- Retry mechanism for failed sends
- Email delivery tracking
- Template engine (Razor/Scriban)
- Attachment support
- Multiple language support
- Unsubscribe management

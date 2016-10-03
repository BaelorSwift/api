using static BCrypt.Net.BCrypt;

using System.Threading.Tasks;
using Baelor.Database.Models;
using Baelor.Extensions;
using Baelor.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Baelor.Database.Repositories.Interfaces;
using SendGrid;
using System;
using SharpRaven.Core;
using Microsoft.AspNetCore.Hosting;
using Baelor.Models.Internal;

namespace Baelor.Controllers
{
	[Route("[controller]")]
	public class UsersController : Controller
	{
		private IUserRepository _userRepository;
		private IEmailVerificationRepository _emailVerificationRepository;
		private SendGridClient _sendGridClient;
		private IRavenClient _ravenClient;

		public UsersController(IUserRepository userRepository,
			IEmailVerificationRepository emailVerificationRepository,
			SendGridClient sendGridClient, IRavenClient ravenClient)
		{
			_userRepository = userRepository;
			_emailVerificationRepository = emailVerificationRepository;
			_sendGridClient = sendGridClient;
			_ravenClient = ravenClient;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAsync()
		{
			return Json(await _userRepository.All());
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync([FromBody] CreateUserViewModel model)
		{
			if (!ModelState.IsValid)
				return Json(new Error("validation_failed", ModelState.Errors()));

			// Check if email or username is already in use
			var emailAddressTask = _userRepository.GetByEmailAddress(model.EmailAddress);
			var usernameTask = _userRepository.GetByUsername(model.Username);
			await Task.WhenAll(emailAddressTask, usernameTask);

			if (emailAddressTask.Result != null)
				ModelState.AddModelError(nameof(model.EmailAddress), "This Email Address is already in use.");
			if (usernameTask.Result != null)
				ModelState.AddModelError(nameof(model.Username), "This Username is already in use.");

			if (!ModelState.IsValid)
				return Json(new Error("validation_failed", ModelState.Errors()));

			// Hash Password
			model.Password = HashPassword(model.Password);

			var user = new User(model);
			await _userRepository.Add(user);

			// Create Email Verification Code
			var emailVerification = new EmailVerification(user.Id);
			await _emailVerificationRepository.Add(emailVerification);

			// Send Code to email
			try
			{
				var url = $"/users/verify?code={emailVerification.Code}";
				if (Startup.HostingEnvironment.IsDevelopment())
					url = "http://localhost:3000" + url;
				else
					url = "https://baelor.io" + url;

				await _sendGridClient.MailClient.SendAsync(
					to: user.EmailAddress,
					toName: user.Name,
					subject: "Verify your Baelor.io Account",
					htmlBody: $"To verify your account, please click the following link: <a href=\"{url}\">{url}</a>",
					textBody: $"To verify your account, please following this link: {url}",
					@from: "support@baelor.io",
					fromName: "BaelorFromName.io"
				);
			}
			catch (Exception ex)
			{
				await _ravenClient.CaptureAsync("email_transport_failed", ex);
			}

			return Json(user);
		}
	}
}

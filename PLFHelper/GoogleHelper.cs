/*
 * Copyright (c) 2013 Oliver Schramm
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util;
using Google.Apis.Services;

namespace PLFHelper
{
	/// <summary>
	/// Provides functions for Google. To. Help. You.
	/// </summary>
	internal partial class GoogleHelper
	{
		//
		const string CLIENT_ID = "CLIENT_ID";
		const string CLIENT_SECRET = "CLIENT_SECRET";
		//
		bool authenticated = false;
		DriveService service;
		
		public static string AppDataPath
		{
			get
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				return Path.Combine(appData, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			}
		}

		public GoogleHelper(bool authenticate = false)
		{
			if (authenticate)
			{
				this.service = this.Authenticate();
				this.authenticated = true;
			}
		}

		public void Authenticate()
		{
			if (this.authenticated)
			{
				// Register the authenticator and create the service
				var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, CLIENT_ID, CLIENT_SECRET);
				var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
				this.service = new DriveService(new BaseClientService.Initializer()
				{
					Authenticator = auth,
					ApplicationName = "Preislistenpfleger Helper",
				});
			}
		}

		private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
		{
			// Use a more secure way to save these...
			const string STORAGE = "STORAGE";
			const string KEY = "KEY";
			
			// Get the auth URL:
			IAuthorizationState state = new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
			state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
			Uri authUri = arg.RequestUserAuthorization(state);

			string authCode = "Have to add own request handler";

			return arg.ProcessUserAuthorization(authCode, state);
		}
		
		private AuthorizationState GetCachedRefreshToken(string storageName, string key)
		{
			string file = storageName + ".auth";
			string dir = GoogleHelper.AppDataPath;
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			string filePath = Path.Combine(dir, file);
			if (!File.Exists(filePath))
			{
				return null;
			}
			byte[] content = File.ReadAllBytes(filePath);
			byte[] salt = Encoding.Unicode.GetBytes(System.Reflection.Assembly.GetExecutingAssembly().FullName + key);
			byte[] decrypted = ProtectedData.Unprotect(content, salt, DataProtectionScope.CurrentUser);
			string[] content = Encoding.Unicode.GetString(decrypted).Split(new[] { "\r\n" }, StringSplitOptions.None);
			
			// Create the authorization state
			string[] scopes = content[0].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			string refreshToken = content[1];
			return new AuthorizationState(scopes) { RefreshToken = refreshToken };
		}
	}
}

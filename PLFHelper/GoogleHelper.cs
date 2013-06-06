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
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
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
				return new DriveService(new BaseClientService.Initializer()
				{
					Authenticator = auth;
				});
			}
		}

		private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
		{
			// Get the auth URL:
			IAuthorizationState state = new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
			state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
			Uri authUri = arg.RequestUserAuthorization(state);

			string authCode = "Have to add own request handler";

			return arg.ProcessUserAuthorization(authCode, state);
		}
	}
}

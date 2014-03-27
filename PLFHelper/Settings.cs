/*
 * Copyright (c) 2014 Oliver Schramm
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
using System.Security.Cryptography;
using System.Text;

namespace PLFHelper
{
	/// <summary>
	/// A serzializable settings class.
	/// </summary>
	[Serializable()]
	internal class Settings
	{
		[NonSerialized()]
		private const string addToEntropy = "d793fd196c929c163d66ebe4115bd474d44364f8";
		private bool autoLogin = false;
		private bool autoUpload = false;
		private int lastServer = -1;
		private byte[] password;
		private bool[] remember = { false, false };
		private bool saveServer = false;
		private ServerSettings[] server;
		private byte[] username;

		/// <summary>
		/// Gets if auto login is enabled.
		/// </summary>
		public bool AutoLogin
		{
			get
			{
				return this.autoLogin;
			}

			set
			{
            	this.autoLogin = value;
            }
		}

		/// <summary>
		/// Gets if auto upload is enabled.
		/// </summary>
		public bool AutoUpload
		{
			get
			{
				return this.autoUpload;
			}

			set
			{
				this.autoUpload = value;
			}
		}

		/// <summary>
		/// Gets the last selected server.
		/// </summary>
		public int LastServer
		{
			get
			{
				return this.lastServer;
			}

			set
			{
				this.lastServer = value;
			}
		}

		/// <summary>
		/// Gets the password.
		/// </summary>
		public string Password
		{
			get
			{
				try
				{
					SHA1Managed sha1 = new SHA1Managed();
					string entropy = Encoding.UTF8.GetString(sha1.ComputeHash(this.username)) + "/" + addToEntropy;
					return Encoding.UTF8.GetString(ProtectedData.Unprotect(this.password, sha1.ComputeHash(Encoding.UTF8.GetBytes(entropy)), DataProtectionScope.CurrentUser));
				}
				catch
				{
					return String.Empty;
				}
			}

			set
			{
				SHA1Managed sha1 = new SHA1Managed();
				string entropy = Encoding.UTF8.GetString(sha1.ComputeHash(this.username)) + "/" + addToEntropy;
				this.password = ProtectedData.Protect(Encoding.UTF8.GetBytes(value), sha1.ComputeHash(Encoding.UTF8.GetBytes(entropy)), DataProtectionScope.CurrentUser);
			}
		}

		/// <summary>
		/// Gets the state of both remember checkboxes.
		/// </summary>
		public bool[] Remember
		{
			get
			{
				return this.remember;
			}

			set
			{
				if (value.Length == 2)
				{
					this.remember = value;
				}
				else
				{
					throw new ArgumentException("The array must have two elements.", "value");
				}
			}
		}

		/// <summary>
		/// Gets if server should be saved.
		/// </summary>
		public bool SaveServer
		{
			get
			{
				return this.saveServer;
			}

			set
			{
				this.saveServer = value;
			}
		}

		/// <summary>
		/// Gets the array of ServerSettings objects.
		/// </summary>
		public ServerSettings[] Server
		{
			get
			{
				return this.server;
			}

			set
			{
				if (value != null && value.Length > 0)
				{
					this.server = value;
				}
				else
				{
					throw new ArgumentException("The array may not be null and must have at least one element", "value");
				}
			}
		}

		/// <summary>
		/// Gets the username.
		/// </summary>
		public string Username
		{
			get
			{
				try
				{
					return Encoding.UTF8.GetString(this.username);
				}
				catch
				{
					return String.Empty;
				}
			}

			set
			{
				this.username = Encoding.UTF8.GetBytes(value);
			}
		}

		/// <summary>
		/// Creates a new instance of the Settings class.
		/// </summary>
		/// <param name="remember">Array of <c>boolean</c> values which represent the state of both remember checkboxes.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="autoLogin"><c>true</c> if auto login is enabled, <c>false</c> if not.</param>
		/// <param name="autoUpload"><c>true</c> if auto upload is enabled, <c>false</c> if not.</param>
		/// <param name="saveServer"><c>true</c> if saving server is enabled, <c>false</c> if not.</param>
		/// <param name="lastServer">The last selected server.</param>
		/// <param name="server">Array of ServerSettings objects.</param>
		public Settings(bool[] remember, string username, string password, bool autoLogin, bool autoUpload, bool saveServer, int lastServer, ServerSettings[] server)
		{
			this.Remember = remember;
			this.Username = (this.Remember[0]) ? username : String.Empty;
			this.Password = (this.Remember[1]) ? password : String.Empty;
			this.AutoLogin = autoLogin;
			this.AutoUpload = autoUpload;
			this.SaveServer = saveServer;
			this.LastServer = (this.SaveServer) ? lastServer : -1;
			this.Server = server;
		}
	}
}

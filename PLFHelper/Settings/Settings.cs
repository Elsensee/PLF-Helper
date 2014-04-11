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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;

using PLFHelper.Localization;

namespace PLFHelper.Settings
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
					var sha1 = new SHA1Managed();
					var entropy = Encoding.UTF8.GetString(sha1.ComputeHash(this.username)) + "/" + addToEntropy;
					return Encoding.UTF8.GetString(ProtectedData.Unprotect(this.password, sha1.ComputeHash(Encoding.UTF8.GetBytes(entropy)), DataProtectionScope.CurrentUser));
				}
				catch
				{
					return String.Empty;
				}
			}

			set
			{
				var sha1 = new SHA1Managed();
				var entropy = Encoding.UTF8.GetString(sha1.ComputeHash(this.username)) + "/" + addToEntropy;
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
					throw new ArgumentException(LocalizationManager.GetLocalizedString("ArrayTwoElements"), "value");
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
					throw new ArgumentException(LocalizationManager.GetLocalizedString("ArrayNotNull"), "value");
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

		/// <summary>
		/// Saves the current settings object to <paramref name="path"/>.
		/// </summary>
		/// <param name="path">The path, the current settings object should be saved to.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
		public void Save(string path)
		{
			SaveSettings(this, path);
		}

		/// <summary>
		/// Saves the given <paramref name="settingsObject"/> to <paramref name="path"/>.
		/// </summary>
		/// <param name="settingsObject">The settings object that sould be saved.</param>
		/// <param name="path">The path, the given settings object should be saved to.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="settingsObject"/> or <paramref name="path"/> is null.</exception>
		public static void SaveSettings(Settings settingsObject, string path)
		{
			if (settingsObject == null)
			{
				throw new ArgumentNullException("settingsObject");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			try
			{
				using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 8192, FileOptions.Encrypted))
				{
					// Yup - we don't need that object after that so just... don't save it
					new BinaryFormatter().Serialize(fileStream, settingsObject);
				}
			}
			// Not the finest way, but it's okay to just say nothing...
			// We don't want to bother the user with that.
			catch (SecurityException) { }
		}

		/// <summary>
		/// Loads a settings object from a given <paramref name="path"/>.
		/// </summary>
		/// <param name="path">The path, the settings object should be loaded from.</param>
		/// <returns>A new settings object with the data loaded from the file given in <paramref name="path"/>.</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
		/// <exception cref="System.IO.FileNotFoundException">The file in <paramref name="path"/> does not exist.</exception>
		public static Settings LoadSettings(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			try
			{
				using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, FileOptions.Encrypted))
				{
					// FileStream will be closed anyway.
					return (Settings) new BinaryFormatter().Deserialize(fileStream);
				}
			}
			// See above...
			catch (SecurityException)
			{
				// we have to return something... but we can't do some random settings...
				// so we just return null.
				return null;
			}
		}
	}
}

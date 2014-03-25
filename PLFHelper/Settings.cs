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
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace PLFHelper
{
	/// <summary>
	/// Description of Settings.
	/// </summary>
	[Serializable()]
	internal class Settings
	{
		[NonSerialized()]
		private const string addToEntropy = "d793fd196c929c163d66ebe4115bd474d44364f8";
		private bool autoLogin = false;
		private bool autoUpload = false;
		private int lastServer = -1;
		private LevelSettings[] level;
		private byte[] password;
		private bool[] remember = { false, false };
		private bool saveServer = false;
		private byte[] username;

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

		public LevelSettings[] Level
		{
			get
			{
				return this.level;
			}

			set
			{
				if (value != null && value.Length > 0)
				{
					this.level = value;
				}
				else
				{
					throw new ArgumentException("The array may not be null and must have at least one element", "value");
				}
			}
		}

		public string Password
		{
			get
			{
				try
				{
					SHA1Managed sha1 = new SHA1Managed();
					string entropy = Encoding.UTF8.GetString(sha1.ComputeHash(this.username)); + "/" + addToEntropy;
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
				string entropy = Encoding.UTF8.GetString(sha1.ComputeHash(this.username)); + "/" + addToEntropy;
				this.password = ProtectedData.Protect(Encoding.UTF8.GetBytes(value), sha1.ComputeHash(Encoding.UTF8.GetBytes(entropy)), DataProtectionScope.CurrentUser);
			}
		}

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

		public Settings(bool[] remember, string username, string password, bool autoLogin, bool autoUpload, bool saveServer, int lastServer, LevelSettings[] level)
		{
			this.Remember = remember;
			this.Username = (this.Remember[0]) ? username : String.Empty;
			this.Password = (this.Remember[1]) ? password : String.Empty;
			this.AutoLogin = autoLogin;
			this.AutoUpload = autoUpload;
			this.SaveServer = saveServer;
			this.LastServer = (this.SaveServer) ? lastServer : -1;
			this.Level = level;
		}
	}
}

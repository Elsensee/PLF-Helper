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

namespace PLFHelper.Settings
{
	/// <summary>
	/// Settings per server
	/// </summary>
	[Serializable()]
	internal class ServerSettings
	{
		private int level;
		private int server;

		/// <summary>
		/// Gets/sets the current level.
		/// </summary>
		public int Level
		{
			get
			{
				return this.level;
			}

			set
			{
				if (value > -1)
				{
					this.level = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		/// <summary>
		/// Gets/sets the current server.
		/// </summary>
		public int Server
		{
			get
			{
				return this.server;
			}

			set
			{
				if (value > -1)
				{
					this.server = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		/// <summary>
		/// Gets/sets the current value of the ShowPlants-checkbox.
		/// </summary>
		public bool ShowPlants { get; set; }

		/// <summary>
		/// Creates a new instance of the ServerSettings class.
		/// </summary>
		/// <param name="server">The currently selected server.</param>
		/// <param name="level">The currently selected level.</param>
		/// <param name="showPlants">The current value of the ShowPlants-checkbox.</param>
		public ServerSettings(int server, int level, bool showPlants)
		{
			this.Server = server;
			this.Level = level;
			this.ShowPlants = showPlants;
		}
	}
}

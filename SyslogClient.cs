using System;
using System.Net.Sockets;
using System.Text;
///
/// Cyclone Labs, 2016
/// Author: JD Gershan
///

namespace SuperEasySyslog
{
	public enum SyslogLevel
	{
		EMGCY		= 0,
		ALERT		= 1,
		CRITICAL	= 2,
		ERROR		= 3,
		WARNING		= 4,
		NOTICE		= 5,
		INFO		= 6,
		DEBUG		= 7
	}

	public enum SyslogFacility
	{
		Kernel   = 0,
		User     = 1,
		Mail     = 2,
		Daemon   = 3,
		Auth     = 4,
		Syslog   = 5,
		Lpr      = 6,
		News     = 7,
		UUCP     = 8,
		Cron     = 9,
		Security = 10,
		FTP      = 11,
		NTP      = 12,
		Audit    = 13,
		Alert    = 14,
		Clock    = 15,
		Local1   = 16,
		Local2   = 17,
		Local3   = 18,
		Local4   = 19,
		Local5   = 20,
		Local6   = 21,
		Local7   = 22
	}

	public static class SyslogClient
	{
		private static string				originHost;
		private static string				applicationId;
		private static string				remoteHost;
		private static int  				remotePort;
		private static SyslogLevel			maxLevel;
		private static UdpClient			client      = null;
		private static Object				locker      = new Object();
		private static bool 				initialized = false;
		private static int  				repeatCount = 1;
		private static SyslogLevel			repeatLevel = SyslogLevel.EMGCY;


		public static void Init(string origin, string application, string remote, int port, SyslogLevel max = SyslogLevel.DEBUG)
		{
			originHost    = origin;
			applicationId = application;
			remoteHost    = remote;
			remotePort    = port;
			client        = new UdpClient(remote, port);
			maxLevel      = max; // Max permitted log level anything higher than this (higher level = lower priority) will be ignored
			initialized   = true;
		}

		// Call to set repeats for important messages
		public static void SetRepeat(SyslogLevel level, int count = 3)
		{
			repeatLevel = level;
			repeatCount = count;
		}

		
		// Call with log level from enum
		public static void Send(SyslogLevel level, string message)
		{
			if (!initialized)
				throw new Exception("SyslogClient not initialized. Call Init method before sending: SyslogClient.Init(string origin, string application, string remote, int port, SyslogLevel max)");
			if (level <= maxLevel)
			{
				int priority  = ((int)SyslogFacility.Syslog) * 8 + ((int)level);
				string m      = String.Format("<{0}> {1} {2} {3} {4} {5}", priority, DateTime.Now.ToString("MMM dd HH:mm:ss"), originHost, applicationId, level.ToString(), message);
				SendIt(m, level <= repeatLevel ? repeatCount : 1);
			}
		}

		// Call with log level from string, we will convert to enum value.
		public static void Send(string level, string message)
		{
			SyslogLevel aLevel = SyslogLevel.INFO;
			try
			{
				aLevel = (SyslogLevel)Enum.Parse(typeof(SyslogLevel), level, true);
			}
			catch
			{
				; // Coder error accident forgiveness. Just send with default log level.
			}
			Send(aLevel, message);
		}

		private static void SendIt(string message, int repeat)
		{
			try
			{
				byte[] bytes = Encoding.ASCII.GetBytes(message);
				lock (locker)
				{
					for(int count = 0; count < repeat; count++)
						client.Send(bytes, bytes.Length);
				}
			}
			catch (Exception e)
			{
				throw new Exception("Error sending to Syslog: " + remoteHost + ":" + remotePort + Environment.NewLine + e.Message, e.InnerException);
			}
		}
	}
}	

// SAMPLE CODE: (SEE README FOR MORE INFO)
// -- Setup:
// SyslogClient.Init("MolluscHeim", "Clash of Clams", "logs.mycloud.com", 514, Level.INFO)
// SyslogClient.SetRepeat(SyslogLevel.CRITICAL,3)  // Note this call is optional
//
// -- Write to the log:
// SyslogClient.Send(SyslogLevel.INFO, "Application started");
// SyslogClient.Send("DEBUG", "Request received: " + requestDetails);
// SyslogClient.Send(SyslogLevel.WARNING, "Low memory");
//
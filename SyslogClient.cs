using System;
using System.Net.Sockets;
using System.Text;
///
/// Cyclone Labs, 2016
/// Author: JD Gershan
///

namespace SuperEasySyslog
{
	public enum SyslogSeverity
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
		private static SyslogSeverity			minSeverity;
		private static UdpClient			client      = null;
		private static Object				locker      = new Object();
		private static bool 				initialized = false;
		private static int  				repeatCount = 1;
		private static SyslogSeverity			repeatSeverity = SyslogSeverity.EMGCY;
		private static SyslogFacility			defaultFaciltity;


		public static void Init(string origin, string application, string remote, int port, SyslogSeverity severity = SyslogSeverity.DEBUG, SyslogFacility facility = SyslogFacility.User)
		{
			originHost    = origin;
			applicationId = application;
			remoteHost    = remote;
			remotePort    = port;
			client        = new UdpClient(remote, port);
			minSeverity      = severity; // Minimum severity for logging anything lower than this will be ignored
			defaultFaciltity = facility;
			initialized   = true;
		}

		// Call to set repeats for important messages
		public static void SetRepeat(SyslogSeverity severity, int count = 3)
		{
			repeatSeverity = severity;
			repeatCount = count;
		}

		
		// Call with log level from enum
		public static void Send(SyslogSeverity severity, string message)
		{
			if (!initialized)
				throw new Exception("SyslogClient not initialized. Call Init method before sending: SyslogClient.Init(string origin, string application, string remote, int port, SyslogSeverity severity)");
			if (severity <= minSeverity)
			{
				int priority  = ((int)SyslogFacility.Syslog) * 8 + ((int)severity);
				string m      = String.Format("<{0}> {1} {2} {3} {4} {5}", priority, DateTime.Now.ToString("MMM dd HH:mm:ss"), originHost, applicationId, severity.ToString(), message);
				try
				{
					byte[] bytes = Encoding.ASCII.GetBytes(m);
					lock (locker)
					{
						for (int count = 0; count < (severity <= repeatSeverity ? repeatCount : 1); count++)
							client.Send(bytes, bytes.Length);
					}
				}
				catch (Exception e)
				{
					throw new Exception("Error sending to Syslog: " + remoteHost + ":" + remotePort + Environment.NewLine + e.Message, e.InnerException);
				}
			}
		}

		// Call with log level from string, we will convert to enum value.
		public static void Send(string level, string message)
		{
			SyslogSeverity severity = SyslogSeverity.INFO;
			try
			{
				severity = (SyslogSeverity)Enum.Parse(typeof(SyslogSeverity), level, true);
			}
			catch
			{
				; // Coder error accident forgiveness. Just send with default log level.
			}
			Send(severity, message);
		}
	}
}	

// SAMPLE CODE: (SEE README FOR MORE INFO)
// -- Setup:
// SyslogClient.Init("MolluscHeim", "Clash of Clams", "logs.mycloud.com", 514, SyslogSeverity.INFO, SyslogFacilty.User)
// SyslogClient.SetRepeat(SyslogSeverity.CRITICAL,3)  // Note this call is optional
//
// -- Write to the log:
// SyslogClient.Send(SyslogSeverity.INFO, "Application started");
// SyslogClient.Send("DEBUG", "Request received: " + requestDetails);
// SyslogClient.Send(SyslogSeverity.WARNING, "Low memory");
//
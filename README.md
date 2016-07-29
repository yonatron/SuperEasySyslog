# **SyslogClient**
A syslog client for .Net

1. Add the code to your project either as source code (SyslogClient.cs) or as a reference to supereasysyslog.dll
2. Initialize
3. Start logging.

**USAGE:**

**First call the Init method**

**SyslogClient.Init**( \<hostname where client is running>, \<application name>, \<hostname of syslog server>, [\<syslog server port>], [\<minimum accepted severity level - set it to SyslogLevel.DEBUG for all messages>], [\<facility>]);

Example: ```SyslogClient.Init("MolluscHeim", "Clash of Clams", "logs.mycloud.com", 514, SyslogSeverity.INFO, SyslogFacility.User);```

If you try to send a message without calling Init first, an exception will be thrown.

**OPTIONALLY:** 

Set up a repeated send for high severity messages (since we are sending in non-guaranteed udp)

**SyslogClient.SetRepeat**(\<minimum severity level to repeat>,\<number of repeats>)

Example: ```SyslogClient.SetRepeat(SyslogSeverity.CRITICAL,3);```

**NOW SEND MESSAGES!**

SyslogClient.Send( \<severity level> , \<message>);

**Examples:**

```SyslogClient.Send( SyslogSeverity.INFO, "Application started");```

```SyslogClient.Send( SyslogSeverity.WARNING, "Low memory!");```

[NOTE: You can also use a string to set the message's severity level if you prefer like this:]

```SyslogClient.Send( "DEBUG", "Request received: " + requestDetails);```

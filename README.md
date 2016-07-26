# **SyslogClient**
A static syslog class. 

1. Add the code to your project either as source code (SyslogClient.cs) or as a reference to supereasysyslog.dll
2. Initialize
3. Start logging.

**USAGE:**

**First call the Init method**

**SyslogClient.Init**( \<hostname where client is running>, \<application name>, \<hostname of syslog server>, \<syslog server port>, \<minimum accepted priority level - set it to SyslogLevel.DEBUG for all messages>);

Example: ```SyslogClient.Init("MolluscHeim", "Clash of Clams", "logs.mycloud.com", 514, SyslogLevel.INFO);```

If you try to send a message (see: SyslogClient.Send() below without calling Init first, an exception will be thrown.

**OPTIONALLY:** 

Set up a repeated send for high priority messages (since we are sending in non-guaranteed udp)

**SyslogClient.SetRepeat**(\<log level>,\<number of repeats>)

Example: ```SyslogClient.SetRepeat(SyslogLevel.CRITICAL,3);```

**NOW SEND MESSAGES!**

SyslogClient.Send( \<priority level> , \<message>);

**Examples:**

```SyslogClient.Send( SyslogLevel.INFO, "Application started");```

```SyslogClient.Send( SyslogLevel.WARNING, "Low memory!");```

[NOTE: You can also use a string to set the message's log level if you prefer like this:]

```SyslogClient.Send( "DEBUG", "Request received: " + requestDetails);```

# **SyslogClient**
A static syslog class. 

1. Add this code to your project. 
2. Initialize
3. Start logging.

**USAGE:**

**First call the Init method**

**SyslogClient.Init**( \<server ID where client is running>, \<application name>, \<target IP Address>, \<target port>, \<minimum accepted priority level - set it to Level.DEBUG for all messages>);

Example: SyslogClient.Init("MolluscHeim", "Clash of Clams", "logs.mycloud.com", 514, Level.INFO);

If you try to send a message (see: SyslogClient.Send() below without calling Init first, an exception will be thrown.

**OPTIONALLY:** 

Set up a repeated send for high priority messages (since we are sending in non-guaranteed udp)

**SyslogClient.SetRepeat**(\<log level>,\<number of repeats>)

Example: SyslogClient.SetRepeat(SyslogLevel.CRITICAL,3)


**SEND MESSAGES!**

**SyslogClient.Send**( \<priority level> , \<message>);

**Examples:**

**SyslogClient.Send**( SyslogLevel.INFO, "Application started");

**SyslogClient.Send**( SyslogLevel.WARNING, "Low memory!");

[NOTE: You can also us a string to set the message's log level if you prefer like this:]

**SyslogClient.Send**( "DEBUG", "Request received: " + requestDetails);
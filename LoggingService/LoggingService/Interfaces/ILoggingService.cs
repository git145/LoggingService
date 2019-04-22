using System;
using System.Diagnostics;

namespace LoggingService.Interfaces
{
    public interface ILoggingService
    {
        void WriteToLoggingFile(string loggingMessageType, string loggingMessage);

        void WriteToLoggingFile(string loggingMessageType, string loggingMessage, StackFrame stackFrame);

        void WriteToLoggingFile(string loggingMessageType, string loggingMessage, string stackFrame);

        void WriteToLoggingFile(Exception loggingMessage);
    }
}

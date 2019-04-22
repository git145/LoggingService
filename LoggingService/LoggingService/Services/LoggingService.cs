using LoggingService.Interfaces;
using System.IO;
using System.Diagnostics;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using LoggingService.Models;
using LoggingService.Enums;

namespace LoggingService.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly string _loggingDirectory;

        private readonly LoggingModel _loggingModel = new LoggingModel();

        public LoggingService(IConfiguration configuration)
        {
            configuration.GetSection("Logs").Bind(_loggingModel);

            _loggingDirectory = _loggingModel.Directory;

            CreateLoggingDirectory(_loggingDirectory);

            DeleteOldLogging(_loggingModel.DaysToLog);
        }

        private void CreateLoggingDirectory(string loggingDirectory)
        {
            try
            {
                if (!Directory.Exists(loggingDirectory))
                {
                    Directory.CreateDirectory(loggingDirectory);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void WriteToLoggingFile(string loggingMessageType, string loggingMessage)
        {
            DateTime dateTimeNow = DateTime.Now;

            try
            {
                using (StreamWriter loggingFile = File.AppendText($"{_loggingDirectory}\\{dateTimeNow.ToString("dd.MM.yyyy")}.log"))
                {
                    loggingFile.WriteLine($"{dateTimeNow.ToString("yyyy-MM-dd")} {dateTimeNow.ToString("T")}, {loggingMessageType}, Thread: {Thread.CurrentThread.ManagedThreadId}, {loggingMessage}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void WriteToLoggingFile(string loggingMessageType, string loggingMessage, StackFrame stackFrame)
        {
            DateTime dateTimeNow = DateTime.Now;

            try
            {
                using (StreamWriter loggingFile = File.AppendText($"{_loggingDirectory}\\{dateTimeNow.ToString("dd.MM.yyyy")}.log"))
                {
                    loggingFile.WriteLine($"{dateTimeNow.ToString("yyyy-MM-dd")} {dateTimeNow.ToString("T")}, {loggingMessageType}, {stackFrame.GetFileName()}: Line {stackFrame.GetFileLineNumber()}, Thread: {Thread.CurrentThread.ManagedThreadId}, {loggingMessage}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void WriteToLoggingFile(string loggingMessageType, string loggingMessage, string stackFrame)
        {
            DateTime dateTimeNow = DateTime.Now;

            try
            {
                using (StreamWriter loggingFile = File.AppendText($"{_loggingDirectory}\\{dateTimeNow.ToString("dd.MM.yyyy")}.log"))
                {
                    loggingFile.WriteLine($"{dateTimeNow.ToString("yyyy-MM-dd")} {dateTimeNow.ToString("T")}, {loggingMessageType}, {stackFrame}, Thread: {Thread.CurrentThread.ManagedThreadId}, {loggingMessage}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void WriteToLoggingFile(Exception loggingMessage)
        {
            StackTrace stackTrace = new StackTrace(loggingMessage, true);
            StackFrame codeLine = stackTrace.GetFrame(0);

            DateTime dateTimeNow = DateTime.Now;

            try
            {
                using (StreamWriter loggingFile = File.AppendText($"{_loggingDirectory}\\{dateTimeNow.ToString("dd.MM.yyyy")}.log"))
                {
                    loggingFile.WriteLine($"{dateTimeNow.ToString("yyyy-MM-dd")} {dateTimeNow.ToString("T")}, {LoggingLevelEnum.Issue.ToString()}, {codeLine.GetFileName()}: Line {codeLine.GetFileLineNumber()}, Thread: {Thread.CurrentThread.ManagedThreadId}, {loggingMessage}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void DeleteOldLogging(int daysToLog)
        {
            TimeSpan periodToLog = new TimeSpan(daysToLog, 0, 0, 0);

            DateTime firstDateToLog = DateTime.Now.Subtract(periodToLog);

            try
            {
                foreach (string loggingFile in Directory.GetFiles(_loggingDirectory, "*.*"))
                {
                    string fileNameType = loggingFile.Split("\\")[1];
                    string fileName = fileNameType.Split(".log")[0];

                    DateTime loggingFileDate = DateTime.ParseExact(fileName, "dd.MM.yyyy", null);

                    if (loggingFileDate <= firstDateToLog)
                    {
                        File.Delete(loggingFile);

                        WriteToLoggingFile(LoggingLevelEnum.Okay.ToString(), 
                            $"\"{fileNameType}\" was deleted.", 
                            new StackTrace(0, true).GetFrame(0));
                    }
                    else
                    {
                        WriteToLoggingFile(LoggingLevelEnum.Okay.ToString(), 
                            $"\"{fileNameType}\" was not deleted.", 
                            new StackTrace(0, true).GetFrame(0));
                    }
                }
            }
            catch (Exception e)
            {
                WriteToLoggingFile(e);

                WriteToLoggingFile(LoggingLevelEnum.Issue.ToString(),
                    $"Issue deleting old logs: {e}",
                    new StackTrace(0, true).GetFrame(0));
            }
        }
    }
}

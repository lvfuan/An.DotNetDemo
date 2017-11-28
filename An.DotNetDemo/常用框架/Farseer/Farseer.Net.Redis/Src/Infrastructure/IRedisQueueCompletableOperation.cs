using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Redis.Infrastructure
{
    public interface IRedisQueueCompletableOperation
    {
        void CompleteBytesQueuedCommand(Func<byte[]> bytesReadCommand);
        void CompleteDoubleQueuedCommand(Func<double> doubleReadCommand);
        void CompleteIntQueuedCommand(Func<int> intReadCommand);
        void CompleteLongQueuedCommand(Func<long> longReadCommand);
        void CompleteMultiBytesQueuedCommand(Func<byte[][]> multiBytesReadCommand);
        void CompleteMultiStringQueuedCommand(Func<List<string>> multiStringReadCommand);
        void CompleteStringQueuedCommand(Func<string> stringReadCommand);
        void CompleteVoidQueuedCommand(Action voidReadCommand);
    }
}

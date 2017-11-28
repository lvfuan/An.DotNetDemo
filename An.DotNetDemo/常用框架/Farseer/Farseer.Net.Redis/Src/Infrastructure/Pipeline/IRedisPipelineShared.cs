using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Redis.Infrastructure.Pipeline
{
    public interface IRedisPipelineShared : IDisposable, IRedisQueueCompletableOperation
    {
        void Flush();
        bool Replay();
    }
}

using System;
using System.Threading;
using FS.Configs;

namespace FS.Redis.Internal
{
    /// <summary>
    ///     缓冲池
    /// </summary>
    internal class BufferPool
    {
        private BufferPool() { }

        /// <summary>
        /// 缓冲池大小
        /// </summary>
        private const int PoolSize = 1000; //  1.45MB
        /// <summary>
        /// 缓冲池
        /// </summary>
        private static readonly object[] Pool = new object[PoolSize];

        /// <summary>
        /// 清除缓冲区数据
        /// </summary>
        internal static void Flush()
        {
            for (var i = 0; i < Pool.Length; i++) { Interlocked.Exchange(ref Pool[i], null); }
        }

        /// <summary>
        /// 返回指定大小的缓冲池
        /// </summary>
        /// <param name="bufferSize">缓冲池大小</param>
        /// <returns></returns>
        internal static byte[] GetBuffer(int bufferSize)
        {
            return bufferSize > RedisConfigs.ConfigEntity.BufferPoolMaxSize ? new byte[bufferSize] : GetBuffer();
        }

        /// <summary>
        /// 获取缓冲池的数据
        /// </summary>
        /// <returns></returns>
        internal static byte[] GetBuffer()
        {
            object tmp;
            for (var i = 0; i < Pool.Length; i++)
            {
                // 赋值为null，如果原池中的值不为null，则立即返回
                if ((tmp = Interlocked.Exchange(ref Pool[i], null)) != null) { return (byte[])tmp; }
            }
            // 返回新的池
            return new byte[RedisConfigs.ConfigEntity.BufferLength];
        }

        /// <summary>
        /// 将参数中的buffer复制到新的buffer中，通过设置copyFromIndex、copyBytes决定要复制的起初位置及长度
        /// </summary>
        /// <param name="buffer">要复制的数据</param>
        /// <param name="toFitAtLeastBytes">最大长度</param>
        /// <param name="copyFromIndex">要复制的起始位置</param>
        /// <param name="copyBytes">要复制的长度</param>
        internal static void ResizeAndFlushLeft(ref byte[] buffer, int toFitAtLeastBytes, int copyFromIndex, int copyBytes)
        {
            // try doubling, else match
            var newLength = buffer.Length * 2;
            if (newLength < toFitAtLeastBytes) newLength = toFitAtLeastBytes;

            var newBuffer = new byte[newLength];
            if (copyBytes > 0) { Buffer.BlockCopy(buffer, copyFromIndex, newBuffer, 0, copyBytes); }
            if (buffer.Length == RedisConfigs.ConfigEntity.BufferLength) { ReleaseBufferToPool(ref buffer); }
            buffer = newBuffer;
        }

        /// <summary>
        /// 将buffer值替换到Pool缓冲池中
        /// </summary>
        /// <param name="buffer"></param>
        internal static void ReleaseBufferToPool(ref byte[] buffer)
        {
            if (buffer == null) return;
            if (buffer.Length == RedisConfigs.ConfigEntity.BufferLength)
            {
                for (var i = 0; i < Pool.Length; i++)
                {
                    if (Interlocked.CompareExchange(ref Pool[i], buffer, null) == null)
                    {
                        break; // found a null; swapped it in
                    }
                }
            }
            // if no space, just drop it on the floor
            buffer = null;
        }
    }
}
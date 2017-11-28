using FS.Extends;

namespace FS.Redis.Internal
{
    /// <summary>
    /// Redis执行命令
    /// </summary>
    internal class Commands
    {
        #region Key的管理
        /// <summary> 删除给定的一个或多个 key，不存在的 key 会被忽略，返回值：被删除 key 的数量 </summary>
        public readonly static byte[] Del = "DEL".ToUtf8Bytes();
        /// <summary> 返回 key 所储存的值的类型 </summary>
        public readonly static byte[] Type = "TYPE".ToUtf8Bytes();
        /// <summary> 检查给定 key 是否存在 </summary>
        public readonly static byte[] Exists = "EXISTS".ToUtf8Bytes();
        /// <summary> 查找所有符合给定模式 pattern 的 key；KEYS * 匹配数据库中所有 key；KEYS h?llo 匹配 hello，hallo等。KEYS h[ae]llo匹配hello和hallo </summary>
        public readonly static byte[] Keys = "KEYS".ToUtf8Bytes();
        /// <summary> 从当前数据库中随机返回(已使用的)一个key </summary>
        public readonly static byte[] RandomKey = "RANDOMKEY".ToUtf8Bytes();
        /// <summary> 将Key改名 </summary>
        public readonly static byte[] Rename = "RENAME".ToUtf8Bytes();
        /// <summary> 当且仅当 newkey 不存在时，将 key 改名为 newkey </summary>
        public readonly static byte[] RenameNx = "RENAMENX".ToUtf8Bytes();
        /// <summary> 以秒为单位设置 key 的有效时间 </summary>
        public readonly static byte[] Expire = "EXPIRE".ToUtf8Bytes();
        /// <summary> 以秒为单位设置 key 的有效时间(timespan) </summary>
        public readonly static byte[] ExpireAt = "EXPIREAT".ToUtf8Bytes();
        /// <summary> 将 key 原子性地从当前实例传送到目标实例的指定数据库上，一旦传送成功， key 保证会出现在目标实例上，而当前实例上的 key 会被删除。执行的时候会阻塞进行迁移的两个实例 </summary>
        public readonly static byte[] Migrate = "MIGRATE".ToUtf8Bytes();
        /// <summary> 将当前数据库的 key 移动到给定的数据库 db 当中 </summary>
        public readonly static byte[] Move = "MOVE".ToUtf8Bytes();
        /// <summary> 命令允许从内部察看给定 key 的 Redis 对象。 </summary>
        public readonly static byte[] Object = "OBJECT".ToUtf8Bytes();
        /// <summary> 返回给定 key 引用所储存的值的次数。此命令主要用于除错 </summary>
        public readonly static byte[] ObjectRefcount = "REFCOUNT".ToUtf8Bytes();
        /// <summary> 返回给定 key 锁储存的值所使用的内部表示(representation) </summary>
        public readonly static byte[] ObjectIdletime = "IDLETIME".ToUtf8Bytes();
        /// <summary> 返回给定 key 自储存以来的空转时间(idle， 没有被读取也没有被写入)，以秒为单位。 </summary>
        public readonly static byte[] ObjectEncoding = "ENCODING".ToUtf8Bytes();
        /// <summary> 移除给定 key 的有效时间 </summary>
        public readonly static byte[] Persist = "PERSIST".ToUtf8Bytes();
        /// <summary> 以毫秒为单位设置 key 的有效时间 </summary>
        public readonly static byte[] PExpire = "PEXPIRE".ToUtf8Bytes();
        /// <summary> 以毫秒为单位设置 key 的有效时间(timespan) </summary>
        public readonly static byte[] PExpireAt = "PEXPIREAT".ToUtf8Bytes();
        /// <summary> 以毫秒为单位返回key的剩余有效时间 </summary>
        public readonly static byte[] PTtl = "PTTL".ToUtf8Bytes();
        /// <summary> 反序列化给定的序列化值，并将它和给定的 key 关联 </summary>
        public readonly static byte[] Restore = "RESTORE".ToUtf8Bytes();
        /// <summary> 返回或保存给定列表、集合、有序集合 key 中经过排序的元素 </summary>
        public readonly static byte[] Sort = "SORT".ToUtf8Bytes();
        /// <summary> 以秒为单位，返回给定 key 的剩余有效时间 </summary>
        public readonly static byte[] Ttl = "TTL".ToUtf8Bytes();
        /// <summary> 序列化给定 key，返回被序列化的值，使用 RESTORE 命令可以将这个值反序列化为 Redis 键 </summary>
        public readonly static byte[] Dump = "DUMP".ToUtf8Bytes();
        /// <summary> 增量迭代 </summary>
        public readonly static byte[] Scan = "SCAN".ToUtf8Bytes();
        /// <summary> 排序 </summary>
        public readonly static byte[] By = "BY".ToUtf8Bytes();
        /// <summary> 排序结果保存到新的Key </summary>
        public readonly static byte[] Store = "STORE".ToUtf8Bytes();
        #endregion

        #region String的管理
        /// <summary> 将值追加到指定key的值末尾，如果key不存在，则相当于增加操作。 </summary>
        public readonly static byte[] Append = "APPEND".ToUtf8Bytes();
        /// <summary> 计算给定字符串中，被设置为 1 的Bit位的数量。 </summary>
        public readonly static byte[] BitCount = "BITCOUNT".ToUtf8Bytes();
        /// <summary> 对一个或多个保存二进制位的字符串 key 进行位元操作 </summary>
        public readonly static byte[] Bitop = "BITOP".ToUtf8Bytes();
        /// <summary> 对一个或多个 key 求逻辑并，并将结果保存到 destkey 。 </summary>
        public readonly static byte[] And = "AND".ToUtf8Bytes();
        /// <summary> 对一个或多个 key 求逻辑或，并将结果保存到 destkey 。 </summary>
        public readonly static byte[] Or = "OR".ToUtf8Bytes();
        /// <summary> 对一个或多个 key 求逻辑异或，并将结果保存到 destkey 。 </summary>
        public readonly static byte[] Xor = "XOR".ToUtf8Bytes();
        /// <summary> 对给定 key 求逻辑非，并将结果保存到 destkey 。 </summary>
        public readonly static byte[] Not = "NOT".ToUtf8Bytes();
        /// <summary> 将 key 中储存的数字值减一。Key不存在，则将值置0，key类型不正确返回一个错误 </summary>
        public readonly static byte[] Decr = "DECR".ToUtf8Bytes();
        /// <summary> 将key所储存的值减去指定数量 </summary>
        public readonly static byte[] DecrBy = "DECRBY".ToUtf8Bytes();
        /// <summary> 返回key所关联的字符串值，如果Key储存的值不是字符串类型，返回一个错误。 </summary>
        public readonly static byte[] Get = "GET".ToUtf8Bytes();
        /// <summary> 对key所储存的字符串值，获取指定偏移量上的位 </summary>
        public readonly static byte[] GetBit = "GETBIT".ToUtf8Bytes();
        /// <summary> 返回key中字符串值的子字符串，字符串的截取范围由start和end两个偏移量决定 </summary>
        public readonly static byte[] GetRange = "GETRANGE".ToUtf8Bytes();
        /// <summary> 将给定key的值设为value，并返回key的旧值。非字符串报错。 </summary>
        public readonly static byte[] GetSet = "GETSET".ToUtf8Bytes();
        /// <summary> 将 key 中储存的数字值增一。不能转换为数字则报错。 </summary>
        public readonly static byte[] Incr = "INCR".ToUtf8Bytes();
        /// <summary> 将key所储存的值加上指定增量 </summary>
        public readonly static byte[] IncrBy = "INCRBY".ToUtf8Bytes();
        /// <summary> 为key中所储存的值加上指定的浮点数增量 </summary>
        public readonly static byte[] IncrByFloat = "INCRBYFLOAT".ToUtf8Bytes();
        /// <summary> 返回所有(一个或多个)给定key的值 </summary>
        public readonly static byte[] MGet = "MGET".ToUtf8Bytes();
        /// <summary> 同时设置一个或多个key-value对 </summary>
        public readonly static byte[] MSet = "MSET".ToUtf8Bytes();
        /// <summary> 同时设置一个或多个key-value对，若一个key已被占用，则全部的执行取消。 </summary>
        public readonly static byte[] MSetNx = "MSETNX".ToUtf8Bytes();
        /// <summary> 以毫秒为单位设置 key 的有效时间 </summary>
        public readonly static byte[] PSetEx = "PSETEX".ToUtf8Bytes();
        /// <summary> 将字符串值value关联到key  </summary>
        public readonly static byte[] Set = "SET".ToUtf8Bytes();
        /// <summary> 对key所储存的字符串值，设置或清除指定偏移量上的位(bit) </summary>
        public readonly static byte[] SetBit = "SETBIT".ToUtf8Bytes();
        /// <summary> 将值value关联到 key，并将key的有效时间(秒) </summary>
        public readonly static byte[] SetEx = "SETEX".ToUtf8Bytes();
        /// <summary> 当key未被使用时，设置为指定值 </summary>
        public readonly static byte[] SetNx = "SETNX".ToUtf8Bytes();
        /// <summary> 用value参数覆写(overwrite)给定key所储存的字符串值，从偏移量 offset 开始 </summary>
        public readonly static byte[] SetRange = "SETRANGE".ToUtf8Bytes();
        /// <summary> 返回key所储存的字符串值的长度 </summary>
        public readonly static byte[] StrLen = "STRLEN".ToUtf8Bytes();
        #endregion

        #region Hash的管理
        /// <summary> 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。 </summary>
        public readonly static byte[] HDel = "HDEL".ToUtf8Bytes();
        /// <summary> 查看哈希表 key 中，给定域 field 是否存在 </summary>
        public readonly static byte[] HExists = "HEXISTS".ToUtf8Bytes();
        /// <summary> 返回哈希表 key 中给定域 field 的值 </summary>
        public readonly static byte[] HGet = "HGET".ToUtf8Bytes();
        /// <summary> 返回哈希表 key 中，所有的域和值 </summary>
        public readonly static byte[] HGetAll = "HGETALL".ToUtf8Bytes();
        /// <summary> 为哈希表 key 中的域 field 的值加上指定增量 </summary>
        public readonly static byte[] HIncrBy = "HINCRBY".ToUtf8Bytes();
        /// <summary> 为哈希表 key 中的域 field 加上指定的浮点数增量 </summary>
        public readonly static byte[] HIncrByFloat = "HINCRBYFLOAT".ToUtf8Bytes();
        /// <summary> 返回哈希表 key 中的所有域 </summary>
        public readonly static byte[] HKeys = "HKEYS".ToUtf8Bytes();
        /// <summary> 返回哈希表 key 中域的数量 </summary>
        public readonly static byte[] HLen = "HLEN".ToUtf8Bytes();
        /// <summary> 返回哈希表 key 中，一个或多个给定域的值 </summary>
        public readonly static byte[] HMGet = "HMGET".ToUtf8Bytes();
        /// <summary> 同时将多个 field-value (域-值)对设置到哈希表 key 中 </summary>
        public readonly static byte[] HMSet = "HMSET".ToUtf8Bytes();
        /// <summary> 将哈希表 key 中的域 field 的值设为 value </summary>
        public readonly static byte[] HSet = "HSET".ToUtf8Bytes();
        /// <summary> 当且仅当域 field 不存在时，将哈希表 key 中的域 field 的值设置为 value </summary>
        public readonly static byte[] HSetNx = "HSETNX".ToUtf8Bytes();
        /// <summary> 返回哈希表 key 中所有域的值 </summary>
        public readonly static byte[] HScan = "HSCAN".ToUtf8Bytes();
        /// <summary> 增量迭代 </summary>
        public readonly static byte[] HVals = "HVALS".ToUtf8Bytes();
        #endregion

        #region List的管理
        /// <summary> 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止 </summary>
        public readonly static byte[] BLPop = "BLPOP".ToUtf8Bytes();
        /// <summary> 与BLPOP同义，弹出位置不同 </summary>
        public readonly static byte[] BRPop = "BRPOP".ToUtf8Bytes();
        /// <summary> 当列表 source 为空时， BRPOPLPUSH 命令将阻塞连接，直到等待超时 </summary>
        public readonly static byte[] BRPopLPush = "BRPOPLPUSH".ToUtf8Bytes();
        /// <summary> 返回列表 key 中，下标为 index 的元素 </summary>
        public readonly static byte[] LIndex = "LINDEX".ToUtf8Bytes();
        /// <summary> 将值 value 插入到列表 key 当中 </summary>
        public readonly static byte[] LInsert = "LINSERT".ToUtf8Bytes();
        /// <summary> 返回列表 key 的长度 </summary>
        public readonly static byte[] LLen = "LLEN".ToUtf8Bytes();
        /// <summary> 移除并返回列表 key 的头元素 </summary>
        public readonly static byte[] LPop = "LPOP".ToUtf8Bytes();
        /// <summary> 将一个或多个值 value 插入到列表 key 的表头 </summary>
        public readonly static byte[] LPush = "LPUSH".ToUtf8Bytes();
        /// <summary> 将值 value 插入到列表 key 的表头，当且仅当 key 存在并且是一个列表 </summary>
        public readonly static byte[] LPushHx = "LPUSHX".ToUtf8Bytes();
        /// <summary> 返回列表 key 中指定区间内的元素，区间以偏移量 start 和 stop 指定 </summary>
        public readonly static byte[] LRange = "LRANGE".ToUtf8Bytes();
        /// <summary> 根据参数 count 的值，移除列表中与参数 value 相等的元素 </summary>
        public readonly static byte[] LRem = "LREM".ToUtf8Bytes();
        /// <summary> 将列表 key 下标为 index 的元素的值设置为 value </summary>
        public readonly static byte[] LSet = "LSET".ToUtf8Bytes();
        /// <summary> 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除 </summary>
        public readonly static byte[] LTrim = "LTRIM".ToUtf8Bytes();
        /// <summary> 移除并返回列表 key 的尾元素 </summary>
        public readonly static byte[] RPop = "RPOP".ToUtf8Bytes();
        /// <summary> 命令 RPOPLPUSH 在一个原子时间内，执行两个动作：1、将列表 source 中的最后一个元素(尾元素)弹出，并返回给客户端。2、将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素。 </summary>
        public readonly static byte[] RPopLPush = "RPOPLPUSH".ToUtf8Bytes();
        /// <summary> 将一个或多个值 value 插入到列表 key 的表尾 </summary>
        public readonly static byte[] RPush = "RPUSH".ToUtf8Bytes();
        /// <summary> 将值 value 插入到列表 key 的表尾，当且仅当 key 存在并且是一个列表 </summary>
        public readonly static byte[] RPushHx = "RPUSHX".ToUtf8Bytes();
        #endregion

        #region Set的管理
        /// <summary> 将一个或多个 member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略 </summary>
        public readonly static byte[] SAdd = "SADD".ToUtf8Bytes();
        /// <summary> 返回集合 key 的集合中元素的数量 </summary>
        public readonly static byte[] SCard = "SCARD".ToUtf8Bytes();
        /// <summary> 返回一个集合的全部成员，该集合是所有给定集合之间的差集 </summary>
        public readonly static byte[] SDiff = "SDIFF".ToUtf8Bytes();
        /// <summary> 这个命令的作用和 SDIFF 类似，但它将结果保存到新集合，而不是简单地返回结果集 </summary>
        public readonly static byte[] SDiffStore = "SDIFFSTORE".ToUtf8Bytes();
        /// <summary> 返回一个集合的全部成员，该集合是所有给定集合的交集 </summary>
        public readonly static byte[] SInter = "SINTER".ToUtf8Bytes();
        /// <summary> 与SINTER类似，不过可以指定保存到新集合 </summary>
        public readonly static byte[] SInterStore = "SINTERSTORE".ToUtf8Bytes();
        /// <summary> 判断 member 元素是否集合 key 的成员 </summary>
        public readonly static byte[] SIsMember = "SISMEMBER".ToUtf8Bytes();
        /// <summary> 返回集合 key 中的所有成员 </summary>
        public readonly static byte[] SMembers = "SMEMBERS".ToUtf8Bytes();
        /// <summary>将 member 元素从一个集合移动到另一个集合  </summary>
        public readonly static byte[] SMove = "SMOVE".ToUtf8Bytes();
        /// <summary> 移除并返回集合中的一个随机元素 </summary>
        public readonly static byte[] SPop = "SPOP".ToUtf8Bytes();
        /// <summary> 仅仅返回随机元素，而不对集合进行任何改动，与SPOP的区别在于不移除 </summary>
        public readonly static byte[] SRandMember = "SRANDMEMBER".ToUtf8Bytes();
        /// <summary> 移除集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略 </summary>
        public readonly static byte[] SRem = "SREM".ToUtf8Bytes();
        /// <summary> 返回一个集合的全部成员，该集合是所有给定集合的并集 </summary>
        public readonly static byte[] SUnion = "SUNION".ToUtf8Bytes();
        /// <summary> 与SUNION类似，不过可以指定保存到新集合 </summary>
        public readonly static byte[] SUnionStore = "SUNIONSTORE".ToUtf8Bytes();
        /// <summary> 增量迭代 </summary>
        public readonly static byte[] SScan = "SSCAN".ToUtf8Bytes();
        #endregion

        #region SortedSet的管理
        /// <summary> 将一个或多个 member 元素及其 score 值加入到有序集 key 当中 </summary>
        public readonly static byte[] ZAdd = "ZADD".ToUtf8Bytes();
        /// <summary> 返回有序集 key 的基数 </summary>
        public readonly static byte[] ZCard = "ZCARD".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中， score 值在 min 和 max 之间(包括 score 值等于 min 或 max )的成员的数量 </summary>
        public readonly static byte[] ZCount = "ZCOUNT".ToUtf8Bytes();
        /// <summary> 为有序集 key 的成员 member 的 score 值加上指定增量 </summary>
        public readonly static byte[] ZIncrBy = "ZINCRBY".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中，指定区间内的成员(小到大排列) </summary>
        public readonly static byte[] ZRange = "ZRANGE".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员 </summary>
        public readonly static byte[] ZRangeByScore = "ZRANGEBYSCORE".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递增(从小到大)顺序排列 </summary>
        public readonly static byte[] ZRank = "ZRANK".ToUtf8Bytes();
        /// <summary> 移除有序集 key 中的一个或多个成员，不存在的成员将被忽略 </summary>
        public readonly static byte[] ZRem = "ZREM".ToUtf8Bytes();
        /// <summary> 移除有序集 key 中，指定排名(rank)区间内的所有成员 </summary>
        public readonly static byte[] ZRemRangeByRank = "ZREMRANGEBYRANK".ToUtf8Bytes();
        /// <summary> 移除有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员 </summary>
        public readonly static byte[] ZRemRangeByScore = "ZREMRANGEBYSCORE".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中，指定区间内的成员，成员位置按score大到小排列 </summary>
        public readonly static byte[] ZRevRange = "ZREVRANGE".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中， score 值介于 max 和 min 之间(默认包括等于 max 或 min )的所有的成员。成员按 score 值递减(从大到小)排列 </summary>
        public readonly static byte[] ZRevRangeByScore = "ZREVRANGEBYSCORE".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递减(从大到小)排序 </summary>
        public readonly static byte[] ZRevRank = "ZREVRANK".ToUtf8Bytes();
        /// <summary> 返回有序集 key 中，成员 member 的 score 值 </summary>
        public readonly static byte[] ZScore = "ZSCORE".ToUtf8Bytes();
        /// <summary> 计算给定的一个或多个有序集的并集，其中给定 key 的数量必须以 numkeys 参数指定，并将该并集(结果集)储存到新集合 </summary>
        public readonly static byte[] ZUnionStore = "ZUNIONSTORE".ToUtf8Bytes();
        /// <summary> 计算给定的一个或多个有序集的交集，其中给定 key 的数量必须以 numkeys 参数指定，并将该交集(结果集)储存到新集合 </summary>
        public readonly static byte[] ZInterStore = "ZINTERSTORE".ToUtf8Bytes();
        /// <summary> 增量迭代 </summary>
        public readonly static byte[] ZScan = "ZSCAN".ToUtf8Bytes();
        #endregion

        #region Pub/Sub的管理
        /// <summary> 订阅一个或多个符合给定模式的频道 </summary>
        public readonly static byte[] PSubscribe = "PSUBSCRIBE".ToUtf8Bytes();
        /// <summary> 将信息 message 发送到指定的频道 </summary>
        public readonly static byte[] Publish = "PUBLISH".ToUtf8Bytes();
        /// <summary> PUBSUB 是一个查看订阅与发布系统状态的内省命令 </summary>
        public readonly static byte[] Pubsub = "PUBSUB".ToUtf8Bytes();
        /// <summary> 指示客户端退订所有给定模式 </summary>
        public readonly static byte[] PUnSubscribe = "PUNSUBSCRIBE".ToUtf8Bytes();
        /// <summary> 订阅给定的一个或多个频道的信息 </summary>
        public readonly static byte[] Subscribe = "SUBSCRIBE".ToUtf8Bytes();
        /// <summary> 指示客户端退订给定的频道 </summary>
        public readonly static byte[] UnSubscribe = "UNSUBSCRIBE".ToUtf8Bytes();
        #endregion

        #region Transaction的管理
        /// <summary> 取消事务，放弃执行事务块内的所有命令 </summary>
        public readonly static byte[] Discard = "DISCARD".ToUtf8Bytes();
        /// <summary> 执行所有事务块内的命令 </summary>
        public readonly static byte[] Exec = "EXEC".ToUtf8Bytes();
        /// <summary> 标记一个事务块的开始 </summary>
        public readonly static byte[] Multi = "MULTI".ToUtf8Bytes();
        /// <summary> 取消 WATCH 命令对所有 key 的监视 </summary>
        public readonly static byte[] UnWatch = "UNWATCH".ToUtf8Bytes();
        /// <summary> 监视一个(或多个) key ，如果在事务执行之前这个(或这些) key 被其他命令所改动，那么事务将被打断 </summary>
        public readonly static byte[] Watch = "WATCH".ToUtf8Bytes();
        #endregion

        #region Script的管理
        /// <summary> 通过内置的 Lua 解释器，可以使用 EVAL 命令对 Lua 脚本进行求值 </summary>
        public readonly static byte[] Eval = "EVAL".ToUtf8Bytes();
        /// <summary> 根据给定的 sha1 校验码，对缓存在服务器中的脚本进行求值 </summary>
        public readonly static byte[] EvalSha = "EVALSHA".ToUtf8Bytes();
        /// <summary> 给定一个或多个脚本的 SHA1 校验和，返回一个包含 0 和 1 的列表，表示校验和所指定的脚本是否已经被保存在缓存当中 </summary>
        public readonly static byte[][] ScriptExists = { "SCRIPT".ToUtf8Bytes(), "EXISTS".ToUtf8Bytes() };
        /// <summary> 清除所有 Lua 脚本缓存 </summary>
        public readonly static byte[][] ScriptFlush = { "SCRIPT".ToUtf8Bytes(), "FLUSH".ToUtf8Bytes() };
        /// <summary> 停止当前正在运行的 Lua 脚本，当且仅当这个脚本没有执行过任何写操作时，这个命令才生效。这个命令主要用于终止运行时间过长的脚本 </summary>
        public readonly static byte[][] ScriptKill = { "SCRIPT".ToUtf8Bytes(), "KILL".ToUtf8Bytes() };
        /// <summary> 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本 </summary>
        public readonly static byte[][] ScriptLoad = { "SCRIPT".ToUtf8Bytes(), "LOAD".ToUtf8Bytes() };

        #endregion

        #region Connection的管理
        /// <summary> 密码验证 </summary>
        public readonly static byte[] Auth = "AUTH".ToUtf8Bytes();
        /// <summary> 打印一个特定的信息 message ，测试时使用。 </summary>
        public readonly static byte[] Echo = "ECHO".ToUtf8Bytes();
        /// <summary> 使用客户端向 Redis 服务器发送一个 PING ，如果服务器运作正常的话，会返回一个 PONG，通常用于测试与服务器的连接是否仍然生效，或者用于测量延迟值 </summary>
        public readonly static byte[] Ping = "PING".ToUtf8Bytes();
        /// <summary> 请求服务器关闭与当前客户端的连接 </summary>
        public readonly static byte[] Quit = "QUIT".ToUtf8Bytes();
        /// <summary> 切换到指定的数据库，数据库索引号 index 用数字值指定，以 0 作为起始索引值 </summary>
        public readonly static byte[] Select = "SELECT".ToUtf8Bytes();
        #endregion

        #region Server的管理
        /// <summary> 执行一个 AOF文件 重写操作。重写会创建一个当前 AOF 文件的体积优化版本。 </summary>
        public readonly static byte[] BgRewriteAof = "BGREWRITEAOF".ToUtf8Bytes();
        /// <summary> 在后台异步(Asynchronously)保存当前数据库的数据到磁盘 </summary>
        public readonly static byte[] BgSave = "BGSAVE".ToUtf8Bytes();
        /// <summary> 返回 CLIENT SETNAME 命令为连接设置的名字 </summary>
        public readonly static byte[][] ClientGetName = { "CLIENT".ToUtf8Bytes(), "GETNAME".ToUtf8Bytes() };
        /// <summary> 关闭地址为 ip:port 的客户端 </summary>
        public readonly static byte[][] ClientKill = { "CLIENT".ToUtf8Bytes(), "KILL".ToUtf8Bytes() };
        /// <summary> 以人类可读的格式，返回所有连接到服务器的客户端信息和统计数据 </summary>
        public readonly static byte[][] ClientList = { "CLIENT".ToUtf8Bytes(), "LIST".ToUtf8Bytes() };
        /// <summary> 为当前连接分配一个名字 </summary>
        public readonly static byte[][] ClientSetName = { "CLIENT".ToUtf8Bytes(), "SETNAME".ToUtf8Bytes() };
        /// <summary> CONFIG GET 命令用于取得运行中的 Redis 服务器的配置参数 </summary>
        public readonly static byte[][] ConfigGet = { "CONFIG".ToUtf8Bytes(), "GET".ToUtf8Bytes() };
        /// <summary> 重置 INFO 命令中的某些统计数据 </summary>
        public readonly static byte[][] ConfigResetStat = { "CONFIG".ToUtf8Bytes(), "RESETSTAT".ToUtf8Bytes() };
        /// <summary> CONFIG REWRITE 命令对启动 Redis 服务器时所指定的 redis.conf 文件进行改写 </summary>
        public readonly static byte[][] ConfigreWrite = { "CONFIG".ToUtf8Bytes(), "REWRITE".ToUtf8Bytes() };
        /// <summary> CONFIG SET 命令可以动态地调整 Redis 服务器的配置而无须重启 </summary>
        public readonly static byte[][] ConfigSet = { "CONFIG".ToUtf8Bytes(), "SET".ToUtf8Bytes() };
        /// <summary> 返回当前数据库的 key 的数量 </summary>
        public readonly static byte[] DbSize = "DBSIZE".ToUtf8Bytes();
        /// <summary> DEBUG OBJECT 是一个调试命令，它不应被客户端所使用 </summary>
        public readonly static byte[][] DebugObject = { "DEBUG".ToUtf8Bytes(), "OBJECT".ToUtf8Bytes() };
        /// <summary> 执行一个不合法的内存访问从而让 Redis 崩溃，仅在开发时用于 BUG 模拟 </summary>
        public readonly static byte[][] DebugSegfault = { "DEBUG".ToUtf8Bytes(), "SEGFAULT".ToUtf8Bytes() };
        /// <summary> 清空整个 Redis 服务器的数据(删除所有数据库的所有 key ) </summary>
        public readonly static byte[] FlushAll = "FLUSHALL".ToUtf8Bytes();
        /// <summary> 清空当前数据库中的所有 key </summary>
        public readonly static byte[] FlushDb = "FLUSHDB".ToUtf8Bytes();
        /// <summary> 返回关于 Redis 服务器的各种信息和统计数值 </summary>
        public readonly static byte[] Info = "INFO".ToUtf8Bytes();
        /// <summary> 返回最近一次 Redis 成功将数据保存到磁盘上的时间，以 UNIX 时间戳格式表示 </summary>
        public readonly static byte[] LastSave = "LASTSAVE".ToUtf8Bytes();
        /// <summary> 实时打印出 Redis 服务器接收到的命令，调试用 </summary>
        public readonly static byte[] Monitor = "MONITOR".ToUtf8Bytes();		//missing
        /// <summary> 用于复制功能的内部命令 </summary>
        public readonly static byte[] Psync = "PSYNC".ToUtf8Bytes();
        /// <summary> SAVE 命令执行一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。一般来说，在生产环境很少执行 SAVE 操作，因为它会阻塞所有客户端，保存数据库的任务通常由 BGSAVE 命令异步地执行。然而，如果负责保存数据的后台子进程不幸出现问题时， SAVE 可以作为保存数据的最后手段来使用。 </summary>
        public readonly static byte[] Save = "SAVE".ToUtf8Bytes();
        /// <summary> 停止所有客户端,如果有至少一个保存点在等待，执行 SAVE 命令,如果 AOF 选项被打开，更新 AOF 文件,关闭 redis 服务器(server) </summary>
        public readonly static byte[] Shutdown = "SHUTDOWN".ToUtf8Bytes();
        /// <summary> 用于在 Redis 运行时动态地修改复制(replication)功能的行为 </summary>
        public readonly static byte[] SlaveOf = "SLAVEOF".ToUtf8Bytes();
        /// <summary> 是 Redis 用来记录查询执行时间的日志系统 </summary>
        public readonly static byte[] Slowlog = "SLOWLOG".ToUtf8Bytes();
        /// <summary> 重置 </summary>
        public readonly static byte[] Reset = "RESET".ToUtf8Bytes();
        /// <summary> 长度 </summary>
        public readonly static byte[] Len = "LEN".ToUtf8Bytes();
        /// <summary> 用于复制功能的内部命令 </summary>
        public readonly static byte[] Sync = "SYNC".ToUtf8Bytes();
        /// <summary> 返回当前服务器时间 </summary>
        public readonly static byte[] Time = "TIME".ToUtf8Bytes();
        #endregion

        #region 参数
        /// <summary> 不移除源实例上的 key </summary>
        public readonly static byte[] Copy = "COPY".ToUtf8Bytes();
        /// <summary> 替换目标实例上已存在的 key </summary>
        public readonly static byte[] Replace = "REPLACE".ToUtf8Bytes();
        /// <summary>
        /// 正序
        /// </summary>
        public readonly static byte[] Asc = "ASC".ToUtf8Bytes();
        /// <summary>
        /// 倒序
        /// </summary>
        public readonly static byte[] Desc = "DESC".ToUtf8Bytes();
        /// <summary>
        /// 分页
        /// </summary>
        public readonly static byte[] Limit = "LIMIT".ToUtf8Bytes();
        /// <summary>
        /// 字符串排序时需要使用
        /// </summary>
        public readonly static byte[] Alpha = "ALPHA".ToUtf8Bytes();
        #endregion


        #region 其它
        /// <summary> 通讯格式里的/r/n换行符 </summary>
        public readonly static byte[] EndData = { (byte)'\r', (byte)'\n' };
        public readonly static byte[] NoSave = "NOSAVE".ToUtf8Bytes();
        public readonly static byte[] No = "NO".ToUtf8Bytes();
        public readonly static byte[] One = "ONE".ToUtf8Bytes();
        public readonly static byte[] Sleep = "SLEEP".ToUtf8Bytes();
        public readonly static byte[] IdleTime = "IDLETIME".ToUtf8Bytes();
        public readonly static byte[] Config = "CONFIG".ToUtf8Bytes();			//missing
        public readonly static byte[] Addr = "ADDR".ToUtf8Bytes();
        public readonly static byte[] Id = "ID".ToUtf8Bytes();
        public readonly static byte[] SkipMe = "SKIPME".ToUtf8Bytes();
        public readonly static byte[] Pause = "PAUSE".ToUtf8Bytes();
        public readonly static byte[] Role = "ROLE".ToUtf8Bytes();
        public readonly static byte[] Match = "MATCH".ToUtf8Bytes();
        public readonly static byte[] Count = "COUNT".ToUtf8Bytes();
        public readonly static byte[] PfAdd = "PFADD".ToUtf8Bytes();
        public readonly static byte[] PfCount = "PFCOUNT".ToUtf8Bytes();
        public readonly static byte[] PfMerge = "PFMERGE".ToUtf8Bytes();
        public readonly static byte[] Before = "BEFORE".ToUtf8Bytes();
        public readonly static byte[] After = "AFTER".ToUtf8Bytes();
        public static readonly byte[] ZRangeByLex = "ZRANGEBYLEX".ToUtf8Bytes();
        public static readonly byte[] ZLexCount = "ZLEXCOUNT".ToUtf8Bytes();
        public static readonly byte[] ZRemRangeByLex = "ZREMRANGEBYLEX".ToUtf8Bytes();
        public readonly static byte[] WithScores = "WITHSCORES".ToUtf8Bytes();
        public readonly static byte[] Ex = "EX".ToUtf8Bytes();
        public readonly static byte[] Px = "PX".ToUtf8Bytes();
        public readonly static byte[] Nx = "NX".ToUtf8Bytes();
        public readonly static byte[] Xx = "XX".ToUtf8Bytes();
        public readonly static byte[] Sentinel = "SENTINEL".ToUtf8Bytes();
        public readonly static byte[] Masters = "masters".ToUtf8Bytes();
        public readonly static byte[] Sentinels = "sentinels".ToUtf8Bytes();
        public readonly static byte[] Master = "master".ToUtf8Bytes();
        public readonly static byte[] Slaves = "slaves".ToUtf8Bytes();
        public readonly static byte[] Failover = "failover".ToUtf8Bytes();
        public readonly static byte[] GetMasterAddrByName = "get-master-addr-by-name".ToUtf8Bytes();

        #endregion
        /// <summary> 聚合方式 </summary>
        public readonly static byte[] Aggregate = "Aggregate".ToUtf8Bytes();
        public readonly static byte[] Max = "Max".ToUtf8Bytes();
        public readonly static byte[] Min = "Min".ToUtf8Bytes();
        public readonly static byte[] Sum = "Sum".ToUtf8Bytes();
        public readonly static byte[] Channels = "Channels".ToUtf8Bytes();
        public readonly static byte[] NumSub = "NumSub".ToUtf8Bytes();
        public readonly static byte[] NumPat = "NumPat".ToUtf8Bytes();
    }
}

namespace FS.Redis.Data
{
    /// <summary>
    /// 集合同一个成员多个score值的聚合方式
    /// </summary>
    public enum eumAggregateType
    {
        /// <summary>
        /// 将所有集合中某个成员的 score 值之 和
        /// </summary>
        Sum,
        /// <summary>
        /// 将所有集合中某个成员的 最小 score 值。
        /// </summary>
        Min,
        /// <summary>
        /// 将所有集合中某个成员的 最大 score 值。
        /// </summary>
        Max
    }
}

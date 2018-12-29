namespace FabricDemo.UserService.Options
{
    /// <summary>
    /// 运维通知配置
    /// </summary>
    public class OperationalOptions
    {
        /// <summary>
        /// 接收邮件列表
        /// </summary>
        public string[] ReceiveEmails { get; set; }

        /// <summary>
        /// 接收短信手机号列表
        /// </summary>
        public string[] ReceiveMobiles { get; set; }
    }
}

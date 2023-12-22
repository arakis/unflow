using Unflow.ArticleBlobStorage;
using Unflow.Cli;

public interface IRemoteNntpClient : IDisposable
{
    Task<bool> ConnectAsync(string hostname, int port, bool useSsl);
    RemoteGroupInfo GetGroupInfo(string name);
    RemotePartialHeader[] GetPartialHeaderForGroup(string name, ArticleRange range);
    RemoteGroupInfo[] GetGroups();
    Article GetArticle(string messageId);
}
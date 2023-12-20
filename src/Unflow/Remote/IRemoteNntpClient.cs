using Unflow.Cli;

public interface IRemoteNntpClient
{
    Task<bool> ConnectAsync(string hostname, int port, bool useSsl);
    RemoteGroupInfo GetGroupInfo(string name);
    RemotePartialHeader[] GetPartialHeaderForGroup(string name, ArticleRange range);
}
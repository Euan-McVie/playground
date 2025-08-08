using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Net9;

public class WebSocketRoundTrip
{
    private readonly Memory<byte> _buffer = Encoding.UTF8.GetBytes(
        "Shall I compare thee to a summer’s day?" +
        "Thou art more lovely and more temperate:" +
        "Rough winds do shake the darling buds of May," +
        "And summer’s lease hath all too short a date;" +
        "Sometime too hot the eye of heaven shines," +
        "And often is his gold complexion dimm'd;" +
        "And every fair from fair sometime declines," +
        "By chance or nature’s changing course untrimm'd;" +
        "But thy eternal summer shall not fade," +
        "Nor lose possession of that fair thou ow’st;" +
        "Nor shall death brag thou wander’st in his shade," +
        "When in eternal lines to time thou grow’st:" +
        "So long as men can breathe or eyes can see," +
        "So long lives this, and this gives life to thee.");

    private readonly Memory<byte> _tmp = new byte[1024];

    private WebSocket? _client;
    private WebSocket? _server;

    [GlobalSetup]
    public void Setup()
    {
        using var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        listener.Listen();

#pragma warning disable CA2000 // Dispose objects before losing scope - Disposed by Cleanup via the _client.
        var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#pragma warning restore CA2000 // Dispose objects before losing scope
        client.Connect(listener.LocalEndPoint!);
        var server = listener.Accept();

        _client = WebSocket.CreateFromStream(new NetworkStream(client, ownsSocket: true), new WebSocketCreationOptions { IsServer = false, });
        _server = WebSocket.CreateFromStream(new NetworkStream(server, ownsSocket: true), new WebSocketCreationOptions { IsServer = true });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _server?.Dispose();
    }

    [Benchmark]
    public async Task SendReceive()
    {
        await _client!.SendAsync(_buffer, WebSocketMessageType.Text, true, default)
            .ConfigureAwait(false);

        while (!(await _server!.ReceiveAsync(_tmp, default).ConfigureAwait(false)).EndOfMessage)
        {
            // Continue receiving until the end of the message
        }
    }
}

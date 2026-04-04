using System.Net.Sockets;
using System.Text.Json;

namespace node_daemon.Infrastructure
{
    public class PodmanService
    {
        private readonly HttpClient client;
        private const string targetApiVersion = "v5.8.0"; // Podman API version to target (changing this without updating methods to reflect the specified version can break functionality!)

        public PodmanService()
        {
            string socketPath = Environment.GetEnvironmentVariable("PODMAN_SOCKET")
                    ?? throw new Exception("PODMAN_SOCKET environment variable is not set or is malformed");

            var handler = new SocketsHttpHandler
            {
                ConnectCallback = async (ctx, ct) =>
                {
                    var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                    await socket.ConnectAsync(new UnixDomainSocketEndPoint(socketPath), ct);
                    return new NetworkStream(socket, ownsSocket: true);
                }
            };

            client = new HttpClient(handler)
            {
                BaseAddress = new Uri($"http://127.0.0.1/{targetApiVersion}")
            };
        }

        public async Task<object> GetStatusAsync()
        {
            bool online = false;
            string? error = null;

            try
            {
                var response = await client.GetAsync("_ping");
                online = response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                online = false;
                error = ex.Message;
            }

            return new
            {
                Online = online,
                Error = error,
            };
        }

        public async Task<JsonElement> GetContainersAsync(bool all = true)
        {
            var response = await client.GetAsync($"containers/json?all={all.ToString().ToLower()}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(json);
        }

        public async Task<JsonElement> GetImagesAsync()
        {
            var response = await client.GetAsync("images/json");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(json);
        }
    }
}
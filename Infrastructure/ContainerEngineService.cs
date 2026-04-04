using System.Net.Sockets;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace node_daemon.Infrastructure
{
    public class ContainerEngineService
    {
        private readonly DockerClient client;
        private string engine;

        public ContainerEngineService()
        {
            engine = Environment.GetEnvironmentVariable("CONTAINER_ENGINE")?.ToLower() ?? "docker";

            string socket = "";
            if (engine == "podman")
                socket = Environment.GetEnvironmentVariable("PODMAN_SOCKET")
                   ?? $"unix://{Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR")}/podman/podman.sock";
            else if (engine == "docker")
                socket = Environment.GetEnvironmentVariable("DOCKER_SOCKET")
                         ?? "unix:///var/run/docker.sock";
            else
                Console.WriteLine("Unsupported container engine");

            client = new DockerClientConfiguration(new Uri(socket)).CreateClient();
        }

        public async Task<object> StatusAsync()
        {
            bool online = false;
            string? error = null;

            try
            {
                await client.System.PingAsync();
                online = true;
            }
            catch (Exception ex)
            {
                online = false;
                error = ex.Message;
            }

            return new
            {
                Online = online,
                Engine = engine,
                Error = error,
            };
        }
    }
}
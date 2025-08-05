using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

public class CallHub : Hub
{
    // This method is called by the agent to initiate a call session.
    // It creates a unique group for the call and returns the ID.
    public async Task<string> StartCallSession()
    {
        var callId = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
        await Groups.AddToGroupAsync(Context.ConnectionId, callId);
        return callId;
    }

    // This method is called by the "customer" to join an existing call session.
    // The error "Method does not exist" happens if this function is missing.
    public async Task JoinCallSession(string callId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, callId);
    }

    // This method receives an audio stream from one client (agent or customer)
    // and broadcasts it to everyone else in the same call group.
    public async Task BroadcastAudioStream(string callId, IAsyncEnumerable<byte[]> stream)
    {
        await foreach (var chunk in stream)
        {
            // Send the audio chunk to the group for this specific call.
            // We also send the original sender's ConnectionId so the client-side
            // can filter out their own audio and not hear themselves.
            await Clients.Group(callId).SendAsync("ReceiveAudioChunk", chunk, Context.ConnectionId);
        }
    }
}

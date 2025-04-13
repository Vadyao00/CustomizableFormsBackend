using Microsoft.AspNetCore.SignalR;

namespace CustomizableForms.Persistance.Hubs;

public class CommentsHub : Hub
{
    public async Task JoinTemplateGroup(string templateId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, templateId);
    }
    
    public async Task LeaveTemplateGroup(string templateId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, templateId);
    }
}
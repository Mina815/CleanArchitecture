using System.Security.Claims;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Web.Hubs;

[Authorize(Roles = Roles.Provider)]
public class BookingHub : Hub
{
    private readonly ApplicationDbContext _dbContext;

    public BookingHub(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<JoinedBranchResponse> JoinBranchRoom(int branchId)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new HubException("Unauthorized.");
        }

        var isOwner = await _dbContext.Branches
            .Where(b => b.Id == branchId)
            .AnyAsync(b => b.Center.OwnerId == userId, Context.ConnectionAborted);

        if (!isOwner)
        {
            throw new HubException("Provider does not own this branch.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetBranchGroup(branchId));
        await Clients.Caller.SendAsync("JoinedBranch", new { branchId }, Context.ConnectionAborted);
        return new JoinedBranchResponse(branchId, true);
    }

    public Task LeaveBranchRoom(int branchId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetBranchGroup(branchId));
    }

    public Task<string> Ping()
    {
        return Task.FromResult("pong");
    }

    public static string GetBranchGroup(int branchId) => $"branch:{branchId}";
}

public record JoinedBranchResponse(int BranchId, bool Joined);

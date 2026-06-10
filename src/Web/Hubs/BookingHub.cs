using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Web.Hubs;

[Authorize]
public class BookingHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly IUser _user;

    public BookingHub(ApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task JoinBranchRoom(int branchId)
    {
        var branch = await _context.Branches
            .Include(b => b.Center)
            .FirstOrDefaultAsync(b => b.Id == branchId);

        if (branch is null || branch.Center.OwnerId != _user.Id)
            throw new HubException("Unauthorized to join this branch room.");

        await Groups.AddToGroupAsync(Context.ConnectionId, BranchRoom(branchId));
        await Clients.Caller.SendAsync("JoinedBranch", new { branchId, branchName = branch.Name });
    }

    public async Task LeaveBranchRoom(int branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, BranchRoom(branchId));
    }

    public Task Ping() => Clients.Caller.SendAsync("Pong");

    public static string BranchRoom(int branchId) => $"branch-{branchId}";
}

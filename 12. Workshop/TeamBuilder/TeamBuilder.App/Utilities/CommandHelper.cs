namespace TeamBuilder.App.Utilities
{
    using System.Linq;

    using TeamBuilder.Data;
    using TeamBuilder.Models;

    public static class CommandHelper 
    {
        public static bool IsTeamExisting(string teamName)
        {
            using (var context = new TeamBuilderContext())
            {
                var exists = context.Teams.Any(t => t.Name.Equals(teamName));
                return exists;
            }
        }

        public static bool IsUserExisting(string username)
        {
            using (var context = new TeamBuilderContext())
            {
                var exists = context.Users.Any(u => u.Username.Equals(username) && !u.IsDeleted);
                return exists;
            }
        }

        public static bool IsInviteExisting(string teamName, User user)
        {
            using (var context = new TeamBuilderContext())
            {
                var exists = context.Invitations
                    .Any(i => i.Team.Name.Equals(teamName) && i.InvitedUserId.Equals(user.Id) && i.IsActive);
                return exists;
            }
        }

        public static bool IsMemberOfTeam(string teamName, string username)
        {
            using (var context = new TeamBuilderContext())
            {
                var isMember = context.UserTeams
                    .Any(ut => ut.Team.Name.Equals(teamName) && ut.User.UserTeams.Equals(username));
                return isMember;
            }
        }

        public static bool IsEventExisting(string eventName)
        {
            using (var context = new TeamBuilderContext())
            {
                var exists = context.Events.Any(e => e.Name.Equals(eventName));
                return exists;
            }
        }

        public static bool IsUserCreatorOfEvent(string eventName, User user)
        {
            using (var context = new TeamBuilderContext())
            {
                var isCreator = context.Events
                    .Any(e => e.Name.Equals(eventName) && e.CreatorId.Equals(user.Id));
                return isCreator;
            }
        }

        public static bool IsUserCreatorOfTeam(string teamName, User user)
        {
            using (var context = new TeamBuilderContext())
            {
                var isCreator = context.Teams
                    .Any(e => e.Name.Equals(teamName) && e.CreatorId.Equals(user.Id));
                return isCreator;
            }
        }
    }
}

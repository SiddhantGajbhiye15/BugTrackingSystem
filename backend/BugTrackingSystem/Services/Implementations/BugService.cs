using BugTrackingSystem.DTOs.Bugs;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Repositories.Interfaces;
using BugTrackingSystem.Services.Interfaces;

namespace BugTrackingSystem.Services.Implementations
{
    public class BugService : IBugService
    {
        private readonly IBugRepository _bugRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;

        public BugService(
            IBugRepository bugRepository,
            IProjectRepository projectRepository,
            IProjectMemberRepository projectMemberRepository)
        {
            _bugRepository = bugRepository;
            _projectRepository = projectRepository;
            _projectMemberRepository = projectMemberRepository;
        }

        public async Task<List<BugResponseDto>> GetProjectBugsAsync(
            int projectId,
            BugStatus? status,
            BugPriority? priority,
            int? assignedDeveloperId,
            int currentUserId,
            UserRole currentUserRole)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            await EnsureProjectAccessAsync(
                project,
                currentUserId,
                currentUserRole);

            var bugs = await _bugRepository.GetProjectBugsAsync(
                projectId,
                status,
                priority,
                assignedDeveloperId);

            return bugs
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<List<BugResponseDto>> GetMyAssignedBugsAsync(
            int currentUserId,
            UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.Developer)
            {
                throw new UnauthorizedAccessException(
                    "Only Developers can view assigned bugs.");
            }

            var activeMembership =
                await _projectMemberRepository
                    .GetActiveMembershipByUserIdAsync(currentUserId);

            if (activeMembership == null)
            {
                return new List<BugResponseDto>();
            }

            var bugs =
                await _bugRepository
                    .GetAssignedBugsAsync(currentUserId);

            return bugs
                .Where(b =>
                    b.ProjectId == activeMembership.ProjectId)
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<BugResponseDto> GetBugByIdAsync(
            int bugId,
            int currentUserId,
            UserRole currentUserRole)
        {
            var bug = await _bugRepository.GetByIdAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            await EnsureProjectAccessAsync(
                bug.Project,
                currentUserId,
                currentUserRole);

            return MapToResponseDto(bug);
        }

        public async Task<BugResponseDto> CreateBugAsync(
            int projectId,
            CreateBugRequestDto request,
            int currentUserId,
            UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.Tester)
            {
                throw new UnauthorizedAccessException(
                    "Only Testers can report bugs.");
            }

            ValidateCreateRequest(request);

            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            if (project.Status != ProjectStatus.Active)
            {
                throw new InvalidOperationException(
                    "Bugs can only be reported for an active project.");
            }

            var membership =
                await _projectMemberRepository.GetMembershipAsync(
                    projectId,
                    currentUserId);

            if (membership == null ||
                membership.RemovedDate != null)
            {
                throw new UnauthorizedAccessException(
                    "You are not an active member of this project.");
            }

            if (!membership.User.IsActive ||
                membership.User.Role != UserRole.Tester)
            {
                throw new UnauthorizedAccessException(
                    "Only an active Tester of this project can report bugs.");
            }

            var bug = new Bug
            {
                ProjectId = projectId,
                ReportedByUserId = currentUserId,
                AssignedDeveloperId = null,

                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Type = request.Type,
                Priority = request.Priority,

                Status = BugStatus.Open,

                ExpectedOutput = request.ExpectedOutput.Trim(),
                ActualOutput = request.ActualOutput.Trim(),
                StepsToReproduce =
                    request.StepsToReproduce.Trim(),

                EvidenceLink =
                    string.IsNullOrWhiteSpace(request.EvidenceLink)
                        ? null
                        : request.EvidenceLink.Trim(),

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,

                Project = project,
                ReportedByUser = membership.User
            };

            await _bugRepository.AddAsync(bug);
            await _bugRepository.SaveChangesAsync();

            return MapToResponseDto(bug);
        }

        public async Task<BugResponseDto> UpdateBugAsync(
            int bugId,
            UpdateBugRequestDto request,
            int currentUserId,
            UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.Tester)
            {
                throw new UnauthorizedAccessException(
                    "Only Testers can update bug details.");
            }

            ValidateUpdateRequest(request);

            var bug = await _bugRepository.GetByIdAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            await EnsureProjectAccessAsync(
                bug.Project,
                currentUserId,
                currentUserRole);

            if (bug.ReportedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You can update only bugs reported by you.");
            }

            if (bug.AssignedDeveloperId != null ||
                bug.Status != BugStatus.Open)
            {
                throw new InvalidOperationException(
                    "A bug can only be edited before it is assigned.");
            }

            bug.Title = request.Title.Trim();
            bug.Description = request.Description.Trim();
            bug.Type = request.Type;
            bug.Priority = request.Priority;

            bug.ExpectedOutput =
                request.ExpectedOutput.Trim();

            bug.ActualOutput =
                request.ActualOutput.Trim();

            bug.StepsToReproduce =
                request.StepsToReproduce.Trim();

            bug.EvidenceLink =
                string.IsNullOrWhiteSpace(request.EvidenceLink)
                    ? null
                    : request.EvidenceLink.Trim();

            bug.UpdatedAt = DateTime.UtcNow;

            await _bugRepository.SaveChangesAsync();

            return MapToResponseDto(bug);
        }

        public async Task<BugResponseDto> AssignBugAsync(
            int bugId,
            AssignBugRequestDto request,
            int currentUserId,
            UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.ProjectManager)
            {
                throw new UnauthorizedAccessException(
                    "Only Project Managers can assign bugs.");
            }

            if (request.DeveloperId <= 0)
            {
                throw new ArgumentException(
                    "A valid developer ID is required.");
            }

            var bug = await _bugRepository.GetByIdAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            EnsureProjectManagerOwnsProject(
                bug.Project,
                currentUserId);

            if (bug.Project.Status != ProjectStatus.Active)
            {
                throw new InvalidOperationException(
                    "Bugs cannot be assigned in an inactive project.");
            }

            if (bug.Status == BugStatus.Resolved ||
                bug.Status == BugStatus.Closed)
            {
                throw new InvalidOperationException(
                    "A resolved or closed bug cannot be assigned.");
            }

            var developerMembership =
                await _projectMemberRepository.GetMembershipAsync(
                    bug.ProjectId,
                    request.DeveloperId);

            if (developerMembership == null ||
                developerMembership.RemovedDate != null)
            {
                throw new InvalidOperationException(
                    "The selected Developer is not an active member of this project.");
            }

            if (!developerMembership.User.IsActive)
            {
                throw new InvalidOperationException(
                    "An inactive Developer cannot be assigned.");
            }

            if (developerMembership.User.Role != UserRole.Developer)
            {
                throw new InvalidOperationException(
                    "The selected user is not a Developer.");
            }

            bug.AssignedDeveloperId = request.DeveloperId;
            bug.AssignedDeveloper = developerMembership.User;
            bug.Status = BugStatus.Assigned;
            bug.UpdatedAt = DateTime.UtcNow;

            await _bugRepository.SaveChangesAsync();

            return MapToResponseDto(bug);
        }

        public async Task<BugResponseDto> ChangeBugStatusAsync(
            int bugId,
            ChangeBugStatusRequestDto request,
            int currentUserId,
            UserRole currentUserRole)
        {
            if (!Enum.IsDefined(
                    typeof(BugStatus),
                    request.Status))
            {
                throw new ArgumentException(
                    "Invalid bug status.");
            }

            var bug = await _bugRepository.GetByIdAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            if (bug.Project.Status != ProjectStatus.Active)
            {
                throw new InvalidOperationException(
                    "The status of a bug in an inactive project cannot be changed.");
            }

            if (currentUserRole == UserRole.Developer)
            {
                await ChangeStatusAsDeveloperAsync(
                    bug,
                    request.Status,
                    currentUserId);
            }
            else if (currentUserRole == UserRole.Tester)
            {
                await ChangeStatusAsTesterAsync(
                    bug,
                    request.Status,
                    currentUserId);
            }
            else
            {
                throw new UnauthorizedAccessException(
                    "Your role cannot change the bug status.");
            }

            bug.UpdatedAt = DateTime.UtcNow;

            await _bugRepository.SaveChangesAsync();

            return MapToResponseDto(bug);
        }

        public async Task<BugResponseDto> ChangeBugPriorityAsync(
            int bugId,
            ChangeBugPriorityRequestDto request,
            int currentUserId,
            UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.ProjectManager)
            {
                throw new UnauthorizedAccessException(
                    "Only Project Managers can change bug priority.");
            }

            if (!Enum.IsDefined(
                    typeof(BugPriority),
                    request.Priority))
            {
                throw new ArgumentException(
                    "Invalid bug priority.");
            }

            var bug = await _bugRepository.GetByIdAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            EnsureProjectManagerOwnsProject(
                bug.Project,
                currentUserId);

            if (bug.AssignedDeveloperId != null ||
                bug.Status != BugStatus.Open)
            {
                throw new InvalidOperationException(
                    "Bug priority can only be changed before assignment.");
            }

            bug.Priority = request.Priority;
            bug.UpdatedAt = DateTime.UtcNow;

            await _bugRepository.SaveChangesAsync();

            return MapToResponseDto(bug);
        }

        public async Task DeleteBugAsync(
            int bugId,
            int currentUserId,
            UserRole currentUserRole)
        {
            var bug = await _bugRepository.GetByIdAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            bool isProjectManager =
                currentUserRole == UserRole.ProjectManager &&
                bug.Project.ProjectManagerId == currentUserId;

            bool isReportingTester =
                currentUserRole == UserRole.Tester &&
                bug.ReportedByUserId == currentUserId;

            if (!isProjectManager && !isReportingTester)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to delete this bug.");
            }

            if (bug.AssignedDeveloperId != null ||
                bug.Status != BugStatus.Open)
            {
                throw new InvalidOperationException(
                    "A bug can only be deleted before assignment.");
            }

            bool hasComments =
                await _bugRepository.HasCommentsAsync(bugId);

            if (hasComments)
            {
                throw new InvalidOperationException(
                    "A bug containing comments cannot be deleted.");
            }

            _bugRepository.Delete(bug);
            await _bugRepository.SaveChangesAsync();
        }

        private async Task EnsureProjectAccessAsync(
            Project project,
            int currentUserId,
            UserRole currentUserRole)
        {
            if (currentUserRole == UserRole.Admin)
            {
                return;
            }

            if (currentUserRole == UserRole.ProjectManager &&
                project.ProjectManagerId == currentUserId)
            {
                return;
            }

            var membership =
                await _projectMemberRepository.GetMembershipAsync(
                    project.ProjectId,
                    currentUserId);

            if (membership == null ||
                membership.RemovedDate != null)
            {
                throw new UnauthorizedAccessException(
                    "You are not an active member of this project.");
            }
        }

        private static void EnsureProjectManagerOwnsProject(
            Project project,
            int currentUserId)
        {
            if (project.ProjectManagerId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the current Project Manager can perform this operation.");
            }
        }

        private async Task ChangeStatusAsDeveloperAsync(
            Bug bug,
            BugStatus requestedStatus,
            int currentUserId)
        {
            var membership =
                await _projectMemberRepository.GetMembershipAsync(
                    bug.ProjectId,
                    currentUserId);

            if (membership == null ||
                membership.RemovedDate != null)
            {
                throw new UnauthorizedAccessException(
                    "You are not an active member of this project.");
            }

            if (bug.AssignedDeveloperId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You can change only bugs assigned to you.");
            }

            bool validTransition =
                (bug.Status == BugStatus.Assigned &&
                 requestedStatus == BugStatus.InProgress)
                ||
                (bug.Status == BugStatus.InProgress &&
                 requestedStatus == BugStatus.Resolved);

            if (!validTransition)
            {
                throw new InvalidOperationException(
                    $"A Developer cannot change status from {bug.Status} to {requestedStatus}.");
            }

            bug.Status = requestedStatus;
        }

        private async Task ChangeStatusAsTesterAsync(
            Bug bug,
            BugStatus requestedStatus,
            int currentUserId)
        {
            var membership =
                await _projectMemberRepository.GetMembershipAsync(
                    bug.ProjectId,
                    currentUserId);

            if (membership == null ||
                membership.RemovedDate != null)
            {
                throw new UnauthorizedAccessException(
                    "You are not an active member of this project.");
            }

            bool validTransition =
                bug.Status == BugStatus.Resolved &&
                (requestedStatus == BugStatus.Closed ||
                 requestedStatus == BugStatus.Reopened);

            if (!validTransition)
            {
                throw new InvalidOperationException(
                    $"A Tester cannot change status from {bug.Status} to {requestedStatus}.");
            }

            bug.Status = requestedStatus;
        }

        private static void ValidateCreateRequest(
            CreateBugRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException(
                    "Bug title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException(
                    "Bug description is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ExpectedOutput))
            {
                throw new ArgumentException(
                    "Expected output is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ActualOutput))
            {
                throw new ArgumentException(
                    "Actual output is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    request.StepsToReproduce))
            {
                throw new ArgumentException(
                    "Steps to reproduce are required.");
            }

            if (!Enum.IsDefined(
                    typeof(BugType),
                    request.Type))
            {
                throw new ArgumentException(
                    "Invalid bug type.");
            }

            if (!Enum.IsDefined(
                    typeof(BugPriority),
                    request.Priority))
            {
                throw new ArgumentException(
                    "Invalid bug priority.");
            }
        }

        private static void ValidateUpdateRequest(
            UpdateBugRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.ExpectedOutput) ||
                string.IsNullOrWhiteSpace(request.ActualOutput) ||
                string.IsNullOrWhiteSpace(
                    request.StepsToReproduce))
            {
                throw new ArgumentException(
                    "All required bug fields must be provided.");
            }

            if (!Enum.IsDefined(
                    typeof(BugType),
                    request.Type))
            {
                throw new ArgumentException(
                    "Invalid bug type.");
            }

            if (!Enum.IsDefined(
                    typeof(BugPriority),
                    request.Priority))
            {
                throw new ArgumentException(
                    "Invalid bug priority.");
            }
        }

        private static BugResponseDto MapToResponseDto(
            Bug bug)
        {
            return new BugResponseDto
            {
                BugId = bug.BugId,
                ProjectId = bug.ProjectId,

                ProjectCode = bug.Project.ProjectCode,
                ProjectName = bug.Project.ProjectName,

                Title = bug.Title,
                Description = bug.Description,

                Type = bug.Type.ToString(),
                Priority = bug.Priority.ToString(),
                Status = bug.Status.ToString(),

                ExpectedOutput = bug.ExpectedOutput,
                ActualOutput = bug.ActualOutput,
                StepsToReproduce = bug.StepsToReproduce,
                EvidenceLink = bug.EvidenceLink,

                ReportedByUserId = bug.ReportedByUserId,

                ReporterName =
                    $"{bug.ReportedByUser.FirstName} " +
                    $"{bug.ReportedByUser.LastName}",

                AssignedDeveloperId =
                    bug.AssignedDeveloperId,

                AssignedDeveloperName =
                    bug.AssignedDeveloper == null
                        ? null
                        : $"{bug.AssignedDeveloper.FirstName} " +
                          $"{bug.AssignedDeveloper.LastName}",

                CreatedAt = bug.CreatedAt,
                UpdatedAt = bug.UpdatedAt
            };
        }
    }
}
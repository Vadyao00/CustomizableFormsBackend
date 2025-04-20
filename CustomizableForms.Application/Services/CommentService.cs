// using AutoMapper;
// using Contracts.IRepositories;
// using Contracts.IServices;
// using CustomizableForms.Domain.DTOs;
// using CustomizableForms.Domain.Entities;
// using CustomizableForms.Domain.Responses;
// using CustomizableForms.LoggerService;
// using CustomizableForms.Persistance.Hubs;
// using Microsoft.AspNetCore.SignalR;
//
// namespace CustomizableForms.Application.Services;
//
// public class CommentService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IHubContext<CommentsHub> hubContext)
//     : ICommentService
// {
//     public async Task<ApiBaseResponse> GetTemplateCommentsAsync(Guid templateId)
//     {
//         try
//         {
//             var template = await repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
//             if (template == null)
//             {
//                 return new ApiBadRequestResponse("Template not found");
//             }
//
//             var comments = await repository.Comment.GetTemplateCommentsAsync(templateId, trackChanges: false);
//             var commentsDto = mapper.Map<IEnumerable<CommentDto>>(comments);
//
//             return new ApiOkResponse<IEnumerable<CommentDto>>(commentsDto);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError($"Error in {nameof(GetTemplateCommentsAsync)}: {ex.Message}");
//             return new ApiBadRequestResponse($"Error retrieving template comments: {ex.Message}");
//         }
//     }
//
//     public async Task<ApiBaseResponse> AddCommentAsync(Guid templateId, CommentForCreationDto commentDto, User currentUser)
//     {
//         try
//         {
//             var template = await repository.Template.GetTemplateByIdAsync(templateId, trackChanges: false);
//             if (template == null)
//             {
//                 return new ApiBadRequestResponse("Template not found");
//             }
//
//             bool isAdmin = false;
//             var userRoles = await repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
//             isAdmin = userRoles.Any(r => r.Name == "Admin");
//
//             if (!template.IsPublic && 
//                 template.CreatorId != currentUser.Id && 
//                 !template.AllowedUsers.Any(au => au.UserId == currentUser.Id) &&
//                 !isAdmin)
//             {
//                 return new ApiBadRequestResponse("You do not have permission to comment on this template");
//             }
//
//             var comment = new TemplateComment
//             {
//                 Id = Guid.NewGuid(),
//                 Content = commentDto.Content,
//                 CreatedAt = DateTime.UtcNow,
//                 TemplateId = templateId,
//                 UserId = currentUser.Id
//             };
//
//             repository.Comment.CreateComment(comment);
//             await repository.SaveAsync();
//
//             var createdComment = await repository.Comment.GetCommentByIdAsync(comment.Id, trackChanges: false);
//             var commentResultDto = mapper.Map<CommentDto>(createdComment);
//
//             await hubContext.Clients.Group(templateId.ToString())
//                 .SendAsync("ReceiveComment", commentResultDto);
//             
//             return new ApiOkResponse<CommentDto>(commentResultDto);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError($"Error in {nameof(AddCommentAsync)}: {ex.Message}");
//             return new ApiBadRequestResponse($"Error adding comment: {ex.Message}");
//         }
//     }
//
//     public async Task<ApiBaseResponse> UpdateCommentAsync(Guid commentId, CommentForUpdateDto commentDto, User currentUser)
//     {
//         try
//         {
//             var comment = await repository.Comment.GetCommentByIdAsync(commentId, trackChanges: true);
//             if (comment == null)
//             {
//                 return new ApiBadRequestResponse("Comment not found");
//             }
//
//             bool isAdmin = false;
//             var userRoles = await repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
//             isAdmin = userRoles.Any(r => r.Name == "Admin");
//
//             if (comment.UserId != currentUser.Id && !isAdmin)
//             {
//                 return new ApiBadRequestResponse("You do not have permission to update this comment");
//             }
//
//             comment.Content = commentDto.Content;
//             
//             repository.Comment.UpdateComment(comment);
//             await repository.SaveAsync();
//             
//             var updatedComment = await repository.Comment.GetCommentByIdAsync(commentId, trackChanges: false);
//             var commentResultDto = mapper.Map<CommentDto>(updatedComment);
//
//             await hubContext.Clients.Group(comment.TemplateId.ToString())
//                 .SendAsync("UpdateComment", commentResultDto);
//             
//             return new ApiOkResponse<bool>(true);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError($"Error in {nameof(UpdateCommentAsync)}: {ex.Message}");
//             return new ApiBadRequestResponse($"Error updating comment: {ex.Message}");
//         }
//     }
//
//     public async Task<ApiBaseResponse> DeleteCommentAsync(Guid commentId, User currentUser)
//     {
//         try
//         {
//             var comment = await repository.Comment.GetCommentByIdAsync(commentId, trackChanges: true);
//             if (comment == null)
//             {
//                 return new ApiBadRequestResponse("Comment not found");
//             }
//     
//             bool isAdmin = false;
//             var userRoles = await repository.Role.GetUserRolesAsync(currentUser.Id, trackChanges: false);
//             isAdmin = userRoles.Any(r => r.Name == "Admin");
//
//             if (comment.UserId != currentUser.Id && comment.Template.CreatorId != currentUser.Id && !isAdmin)
//             {
//                 return new ApiBadRequestResponse("You do not have permission to delete this comment");
//             }
//
//             var templateId = comment.TemplateId;
//         
//             repository.Comment.DeleteComment(comment);
//             await repository.SaveAsync();
//
//             await hubContext.Clients.Group(templateId.ToString())
//                 .SendAsync("DeleteComment", commentId);
//
//             return new ApiOkResponse<bool>(true);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError($"Error in {nameof(DeleteCommentAsync)}: {ex.Message}");
//             return new ApiBadRequestResponse($"Error deleting comment: {ex.Message}");
//         }
//     }
// }
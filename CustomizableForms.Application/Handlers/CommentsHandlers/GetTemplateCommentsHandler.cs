using AutoMapper;
using Contracts.IRepositories;
using CustomizableForms.Application.Queries.CommentsQueries;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Responses;
using CustomizableForms.LoggerService;
using CustomizableForms.Persistance.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CustomizableForms.Application.Handlers.CommentsHandlers;

public sealed class GetTemplateCommentsHandler(
    IRepositoryManager repository,
    ILoggerManager logger,
    IMapper mapper)
    : IRequestHandler<GetTemplateCommentsQuery, ApiBaseResponse>
{
    public async Task<ApiBaseResponse> Handle(GetTemplateCommentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.Template.GetTemplateByIdAsync(request.TemplateId, trackChanges: false);
            if (template == null)
            {
                return new ApiBadRequestResponse("Template not found");
            }   

            var comments = await repository.Comment.GetTemplateCommentsAsync(request.TemplateId, trackChanges: false);
            var commentsDto = mapper.Map<IEnumerable<CommentDto>>(comments);

            return new ApiOkResponse<IEnumerable<CommentDto>>(commentsDto);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in handler{nameof(GetTemplateCommentsHandler)}: {ex.Message}");
            return new ApiBadRequestResponse($"Error retrieving template comments: {ex.Message}");
        }
    }
}
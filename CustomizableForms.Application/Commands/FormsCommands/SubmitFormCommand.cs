using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.FormsCommands;

public sealed record SubmitFormCommand(Guid TemplateId, FormForSubmissionDto FormDto, User CurrentUser) : IRequest<ApiBaseResponse>;
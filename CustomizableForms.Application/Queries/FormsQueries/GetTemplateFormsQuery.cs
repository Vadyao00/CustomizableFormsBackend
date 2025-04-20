using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.FormsQueries;

public sealed record GetTemplateFormsQuery(FormParameters FormParameters, Guid TemplateId, User CurrentUser) : IRequest<ApiBaseResponse>;
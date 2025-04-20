using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.RequestFeatures;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Queries.FormsQueries;

public sealed record GetUserFormsQuery(FormParameters FormParameters, User CurrentUser): IRequest<ApiBaseResponse>;
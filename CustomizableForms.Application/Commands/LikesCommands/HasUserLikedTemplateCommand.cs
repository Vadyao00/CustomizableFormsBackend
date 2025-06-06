﻿using CustomizableForms.Domain.Entities;
using CustomizableForms.Domain.Responses;
using MediatR;

namespace CustomizableForms.Application.Commands.LikesCommands;

public sealed record HasUserLikedTemplateCommand(Guid TemplateId, User CurrentUser) : IRequest<ApiBaseResponse>;
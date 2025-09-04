// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System.Security.Claims;
global using ArtStore.Application.Common.Interfaces;
global using ArtStore.Application.Common.Interfaces.Identity;
global using ArtStore.Domain.Entities;
global using ArtStore.Infrastructure.Persistence;
global using ArtStore.Infrastructure.Persistence.Extensions;
global using ArtStore.Infrastructure.Services;
global using ArtStore.Infrastructure.Services.Identity;
global using FluentValidation;
global using FluentValidation.Internal;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

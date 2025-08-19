// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ArtStore.Shared.DTOs.Tenant;

[Description("Tenants")]
public class TenantDto
{
    [Description("Tenant Id")] public int Id { get; set; }
    [Description("Tenant Name")] public string? Name { get; set; }
    [Description("Description")] public string? Description { get; set; }
}
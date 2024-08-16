using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class SchemaMigration
{
    public string Version { get; set; } = null!;
}

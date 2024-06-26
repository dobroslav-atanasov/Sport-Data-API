﻿namespace SportHub.Data.Common.Models;

using System;

using SportHub.Data.Common.Interfaces;

public abstract class BaseDeletableEntity<TKey> : BaseEntity<TKey>, IDeletableEntity
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }
}
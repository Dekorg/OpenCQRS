﻿using System;
using Weapsy.Cqrs.Domain;

namespace Weapsy.Cqrs.EF.Entities.Factories
{
    public interface IAggregateEntityFactory
    {
        AggregateEntity CreateAggregate<TAggregate>(Guid aggregateRootId) where TAggregate : IAggregateRoot;
    }
}

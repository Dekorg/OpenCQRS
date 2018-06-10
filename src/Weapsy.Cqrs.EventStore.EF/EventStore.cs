﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Weapsy.Cqrs.Domain;
using Weapsy.Cqrs.EventStore.EF.Entities.Factories;

namespace Weapsy.Cqrs.EventStore.EF
{
    public class EventStore : IEventStore
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IAggregateEntityFactory _aggregateEntityFactory;
        private readonly IEventEntityFactory _eventEntityFactory;

        public EventStore(IDbContextFactory dbContextFactory,
            IAggregateEntityFactory aggregateEntityFactory,
            IEventEntityFactory eventEntityFactory)
        {
            _dbContextFactory = dbContextFactory;
            _aggregateEntityFactory = aggregateEntityFactory;
            _eventEntityFactory = eventEntityFactory;            
        }

        public async Task SaveEventAsync<TAggregate>(IDomainEvent @event) where TAggregate : IAggregateRoot
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var aggregateEntity = await dbContext.Aggregates.FirstOrDefaultAsync(x => x.Id == @event.AggregateRootId);               
                if (aggregateEntity == null)
                {
                    var newAggregateEntity = _aggregateEntityFactory.CreateAggregate<TAggregate>(@event);
                    dbContext.Aggregates.Add(newAggregateEntity);
                }

                var currentSequenceCount = await dbContext.Events.CountAsync(x => x.AggregateId == @event.AggregateRootId);
                var newEventEntity = _eventEntityFactory.CreateEvent(@event, currentSequenceCount + 1);
                dbContext.Events.Add(newEventEntity);

                await dbContext.SaveChangesAsync();
            }
        }

        public void SaveEvent<TAggregate>(IDomainEvent @event) where TAggregate : IAggregateRoot
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var aggregateEntity = dbContext.Aggregates.FirstOrDefault(x => x.Id == @event.AggregateRootId);
                if (aggregateEntity == null)
                {
                    var newAggregateEntity = _aggregateEntityFactory.CreateAggregate<TAggregate>(@event);
                    dbContext.Aggregates.Add(newAggregateEntity);
                }

                var currentSequenceCount = dbContext.Events.Count(x => x.AggregateId == @event.AggregateRootId);
                var newEventEntity = _eventEntityFactory.CreateEvent(@event, currentSequenceCount + 1);
                dbContext.Events.Add(newEventEntity);

                dbContext.SaveChanges();
            }
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId)
        {
            var result = new List<DomainEvent>();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var events = await dbContext.Events.Where(x => x.AggregateId == aggregateId).ToListAsync();
                foreach (var @event in events)
                {
                    var domainEvent = JsonConvert.DeserializeObject(@event.Data, Type.GetType(@event.Type));
                    result.Add((DomainEvent)domainEvent);
                }
            }

            return result;
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateId)
        {
            var result = new List<DomainEvent>();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var events = dbContext.Events.Where(x => x.AggregateId == aggregateId).ToList();
                foreach (var @event in events)
                {
                    var domainEvent = JsonConvert.DeserializeObject(@event.Data, Type.GetType(@event.Type));
                    result.Add((DomainEvent)domainEvent);
                }
            }

            return result;
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByCommandIdAsync(Guid aggregateId, Guid commandId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainEvent> GetEventsByCommandId(Guid aggregateId, Guid commandId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEvent>> GetEventsByEventTypesAsync(Guid aggregateId, IEnumerable<Type> types)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainEvent> GetEventsByEventTypes(Guid aggregateId, IEnumerable<Type> types)
        {
            throw new NotImplementedException();
        }
    }
}

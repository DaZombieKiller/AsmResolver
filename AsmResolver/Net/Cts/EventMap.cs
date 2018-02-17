﻿using System.Runtime.InteropServices;
using AsmResolver.Collections.Generic;
using AsmResolver.Net.Cts.Collections;
using AsmResolver.Net.Metadata;

namespace AsmResolver.Net.Cts
{
    public class EventMap : MetadataMember<MetadataRow<uint, uint>>
    {
        private readonly LazyValue<TypeDefinition> _parent;

        public EventMap(TypeDefinition parent)
            : base(null, new MetadataToken(MetadataTokenType.EventMap))
        {
            _parent = new LazyValue<TypeDefinition>(parent);
            Events = new DelegatedMemberCollection<EventMap, EventDefinition>(this, GetEventOwner, SetEventOwner);
        }


        internal EventMap(MetadataImage image, MetadataRow<uint, uint> row)
            : base(image, row.MetadataToken)
        {
            _parent = new LazyValue<TypeDefinition>(() =>
            {
                var typeTable = image.Header.GetStream<TableStream>().GetTable(MetadataTokenType.TypeDef);
                var typeRow = typeTable.GetRow((int) row.Column1 - 1);
                return (TypeDefinition) typeTable.GetMemberFromRow(image, typeRow);
            });
            
            Events = new RangedMemberCollection<EventMap,EventDefinition>(this, MetadataTokenType.Event, 1, GetEventOwner, SetEventOwner);
        }

        public TypeDefinition Parent
        {
            get { return _parent.Value; }
            set { _parent.Value = value; }
        }

        public Collection<EventDefinition> Events
        {
            get;
            private set;
        }

        private static EventMap GetEventOwner(EventDefinition @event)
        {
            return @event.EventMap;
        }

        private static void SetEventOwner(EventDefinition @event, EventMap owner)
        {
            @event.EventMap = owner;
        }
    }
}
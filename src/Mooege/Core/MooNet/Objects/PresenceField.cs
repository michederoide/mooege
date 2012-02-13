/*
 * Copyright (C) 2011 - 2012 mooege project - http://www.mooege.org
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using Mooege.Core.MooNet.Helpers;
using Google.ProtocolBuffers;
using System.Linq;
using System.Text;

namespace Mooege.Core.MooNet.Objects
{
    public class EntityIdPresenceFieldList : PresenceFieldBase
    {

        private List<bnet.protocol.EntityId> _value = new List<bnet.protocol.EntityId>();
        public List<bnet.protocol.EntityId> Value
        {
            get
            {
                return _value;
            }
            set
            {
                this._value = value;
                this.isSynced = false;
            }
        }

        /// <summary>
        /// C-tor Index is not needed as in enums the index is set to 4 it seems
        /// </summary>
        /// <param name="program"></param>
        /// <param name="originatingClass"></param>
        /// <param name="fieldNumber"></param>
        /// <param name="index"></param>
        public EntityIdPresenceFieldList(FieldKeyHelper.Program program, FieldKeyHelper.OriginatingClass originatingClass, uint fieldNumber)
        {
            FieldNumber = fieldNumber;
            //Index = ?????; //for enums this is a kind of id, maybe presence service id for this enum
            Program = program;
            OriginatingClass = originatingClass;
        }

        public List<bnet.protocol.presence.FieldOperation> GetFieldOperationList()
        {
            var operationList = new List<bnet.protocol.presence.FieldOperation>();

            foreach (var id in Value)
            {
                var Key = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, FieldNumber, id.High);
                var Field = bnet.protocol.presence.Field.CreateBuilder().SetKey(Key).SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetEntityidValue(id).Build()).Build();
                operationList.Add(bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(Field).Build());
            }
            return operationList;
        }
        /// <summary>
        /// Only here to satisfy the base class requirements.
        /// </summary>
        /// <returns></returns>
        public override bnet.protocol.presence.Field GetField()
        {            
            return bnet.protocol.presence.Field.CreateBuilder().Build();
        }

        /// <summary>
        /// Only here to satisfy the baseclass requirements
        /// </summary>
        /// <returns></returns>
        public override bnet.protocol.presence.FieldOperation GetFieldOperation()
        {
            return bnet.protocol.presence.FieldOperation.CreateBuilder().Build();
        }
        
    }

    public class BoolPresenceField : PresenceField<bool>, IPresenceField
    {
        public BoolPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, bool defaultValue = default(bool))
            : base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
        {
        }

        /// <summary>
        /// Delegate to add a specific transformation before sending the Operation
        /// </summary>
        /// <param name="value"></param>
        public delegate bool TransformValueDel(bool a);
        public TransformValueDel transformDelegate = null;

        public override bnet.protocol.presence.Field GetField()
        {
            var _valueToSend = Value;
            // Prepare Value for sending
            // Example computing a hash (see toon.cs persistence fields delegates)
            if (transformDelegate != null)
            {
                _valueToSend = transformDelegate.Invoke((bool)Value);
            }
            var variantValue = bnet.protocol.attribute.Variant.CreateBuilder().SetBoolValue(Value).Build();
            return base.GetField(variantValue);
        }

        public override bnet.protocol.presence.FieldOperation GetFieldOperation()
        {
            var field = GetField();
            return bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build();
        }
    }

    public class UintPresenceField : PresenceField<ulong>, IPresenceField
    {
        public UintPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, ulong defaultValue = default(ulong))
            : base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
        {
        }

        public override bnet.protocol.presence.Field GetField()
        {
            var variantValue = bnet.protocol.attribute.Variant.CreateBuilder().SetUintValue(Value).Build();
            return base.GetField(variantValue);
        }

        public override bnet.protocol.presence.FieldOperation GetFieldOperation()
        {
            var field = GetField();
            return bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build();
        }
    }

    public class IntPresenceField : PresenceField<long>, IPresenceField
    {
        public IntPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, long defaultValue = default(long))
            : base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
        {
        }


        /// <summary>
        /// Delegate to add a specific transformation before sending the Operation
        /// </summary>
        /// <param name="value"></param>
        public delegate int TransformValueDel(int a);
        public TransformValueDel transformDelegate = null;

        public override bnet.protocol.presence.Field GetField()
        {
            var _valueToSend = Value;
            // Prepare Value for sending
            // Example computing a hash (see toon.cs persistence fields delegates)
            if (transformDelegate != null)
            {
                _valueToSend = transformDelegate.Invoke((int)Value);
            }
            var variantValue = bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(_valueToSend).Build();
            return base.GetField(variantValue);
        }

        public override bnet.protocol.presence.FieldOperation GetFieldOperation()
        {
            var field = GetField();
            return bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build();
        }
    }

    public class FourCCPresenceField : PresenceField<String>, IPresenceField
    {
        public FourCCPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, string defaultValue = default(string))
            : base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
        {
        }

        public override bnet.protocol.presence.Field GetField()
        {
            var variantValue = bnet.protocol.attribute.Variant.CreateBuilder().SetFourccValue(Value).Build();
            return base.GetField(variantValue);
        }

        public override bnet.protocol.presence.FieldOperation GetFieldOperation()
        {
            var field = GetField();
            return bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build();
        }
    }

    public class StringPresenceField : PresenceField<String>, IPresenceField
    {
        public StringPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, string defaultValue = default(string))
            :base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
        {
        }

        public override bnet.protocol.presence.Field GetField()
        {
            var variantValue = bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue(Value).Build();
            return base.GetField(variantValue);
        }

        public override bnet.protocol.presence.FieldOperation GetFieldOperation()
        {
            var field = GetField();
            return bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build();
        }
    }

    public class ByteStringPresenceField<T> : PresenceField<T>, IPresenceField where T : IMessageLite<T> //Used IMessageLite to get ToByteString(), might need refactoring later
    {
        public ByteStringPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, T defaultValue = default(T))
            :base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
        {
        }

        public override bnet.protocol.presence.Field GetField()
        {   
            var variantValue = bnet.protocol.attribute.Variant.CreateBuilder();
            if (Value == null)
                variantValue.SetMessageValue(ByteString.Empty);
            else
                variantValue.SetMessageValue(Value.ToByteString());
            return base.GetField(variantValue.Build());
        }

        public override bnet.protocol.presence.FieldOperation GetFieldOperation()
        {

            var field = GetField();
            return bnet.protocol.presence.FieldOperation.CreateBuilder().SetField(field).Build();
        }
    }

    public abstract class PresenceField<T> : PresenceFieldBase
    {

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                //switch flag so on next update call this field will be sent to clients
                isSynced = false;
            }
        }

        public bnet.protocol.presence.Field GetField(bnet.protocol.attribute.Variant variantValue)
        {
            var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
            var field = bnet.protocol.presence.Field.CreateBuilder().SetKey(fieldKey).SetValue(variantValue).Build();
            return field;
        }

        public PresenceField(FieldKeyHelper.Program program, FieldKeyHelper.OriginatingClass originatingClass , uint fieldNumber, uint index, T defaultValue)
        {
            Value = defaultValue;
            FieldNumber = fieldNumber;
            Index = index;
            Program = program;
            OriginatingClass = originatingClass;
        }


    }


    public abstract class PresenceFieldBase: IPresenceField
    {
        //Keeps a record if Value was modified since last update
        public bool isSynced = false;

        public FieldKeyHelper.Program Program { get; set; }
        public FieldKeyHelper.OriginatingClass OriginatingClass { get; set; }
        public uint FieldNumber { get; set; }
        public uint Index { get; set; }

        public bnet.protocol.presence.FieldKey GetFieldKey()
        {
            return FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
        }

        public abstract bnet.protocol.presence.FieldOperation GetFieldOperation();

        public abstract bnet.protocol.presence.Field GetField();
    }

    public interface IPresenceField
    {
        bnet.protocol.presence.Field GetField();
        bnet.protocol.presence.FieldOperation GetFieldOperation();
        bnet.protocol.presence.FieldKey GetFieldKey();
    }
}

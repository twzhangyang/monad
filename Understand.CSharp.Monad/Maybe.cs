using System;

namespace Understand.CSharp.Monad
{
    public class Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly MaybeValueWrapper _value;
        public bool HasValue => _value != null;
        public bool HasNoValue => !HasValue;

        public T Value
        {
            get
            {
                if (HasNoValue)
                {
                    throw new InvalidOperationException();
                }

                return _value.Value;
            }
        }

        private Maybe(T value)
        {
            _value = value == null ? null : new MaybeValueWrapper(value);
        }

        public bool Equals(Maybe<T> other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Maybe<T>) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        private class MaybeValueWrapper
        {
            public MaybeValueWrapper(T value)
            {
                Value = value;
            }

            internal readonly T Value;
        }
    }
}

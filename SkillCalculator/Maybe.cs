using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkillCalculator
{
    public struct Maybe<T>
    {
        public static readonly Maybe<T> Default = new Maybe<T>();

        private readonly T m_item;
        public T Item
        {
            get
            {
                if (HasItem)
                {
                    return m_item;
                }
                else
                {
                    throw new InvalidOperationException("Cannot get non-existent item.");
                }
            }
        }

        public readonly bool HasItem;

        public Maybe(T item)
        {
            m_item = item;
            HasItem = true;
        }

        public static explicit operator T(Maybe<T> t)
        {
            return t.Item;
        }

        public static implicit operator Maybe<T>(T t)
        {
            return new Maybe<T>(t);
        }

        public override int GetHashCode()
        {
            if (!HasItem)
            {
                return 0;
            }

            return Item.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!HasItem)
            {
                return false;
            }

            if (!(obj is Maybe<T>))
            {
                return false;
            }

            var other = (Maybe<T>)obj;
            if (!other.HasItem)
            {
                return false;
            }

            return this.Item.Equals(other.Item);
        }
    }

    public sealed class Maybe
    {
        public static Maybe<T> Create<T>(T item)
        {
            var maybe = new Maybe<T>(item);
            return maybe;
        }

        public static Maybe<T> GetDefault<T>()
        {
            return Maybe<T>.Default;
        }

        public static Maybe<B> Apply<A,B>(Maybe<A> m, Func<A, B> func)
        {
            if (m.HasItem)
            {
                return Create(func(m.Item));
            }
            else
            {
                return Maybe<B>.Default;
            }
        }

        public static A Force<A>(Maybe<A> m)
        {
            if (m.HasItem)
            {
                return m.Item;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}

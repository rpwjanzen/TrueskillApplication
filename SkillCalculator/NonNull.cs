using System;

namespace SkillCalculator
{
    public struct NonNull<T>
    {
        private readonly T m_item;
        public T Item
        {
            get
            {
                if (m_isValid)
                {
                    return m_item;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private bool m_isValid;

        public NonNull(T item)
        {
            if (item == null)
            {
                throw new NotSupportedException("Item must not be null.");
            }
            m_item = item;
            m_isValid = true;
        }

        public static implicit operator T(NonNull<T> t)
        {
            return t.Item;
        }

        public static explicit operator NonNull<T>(T t)
        {
            return new NonNull<T>(t);
        }

        public override bool Equals(object obj)
        {
            if (!m_isValid)
            {
                return false;
            }

            if (!(obj is NonNull<T>))
            {
                return false;
            }

            var other = (NonNull<T>)obj;
            if (!other.m_isValid)
            {
                return false;
            }

            return this.Item.Equals(other.Item);
        }

        public override int GetHashCode()
        {
            if (!m_isValid)
            {
                return 0;
            }

            return Item.GetHashCode();
        }
    }

    public sealed class NonNull
    {
        public NonNull<T> Create<T>(T item)
        {
            return new NonNull<T>(item);
        }

        public static NonNull<B> Apply<A, B>(NonNull<A> m, Func<A, NonNull<B>> func)
        {
            return func(m.Item);
        }
    }
}

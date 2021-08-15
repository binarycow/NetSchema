using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;

namespace NetSchema.Common.Collections
{
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IReadOnlyModuleDictionary<TModule> 
        : IReadOnlyKeyedCollection<XNamespace, TModule>,
            IReadOnlyKeyedCollection<string, TModule>,
            IDisposable
        where TModule : notnull
    {
        
    }
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IModuleDictionary<TModule> 
        : IKeyedCollection<XNamespace, TModule>,
            IKeyedCollection<string, TModule>,
            IReadOnlyModuleDictionary<TModule>
        where TModule : notnull
    {
        
    }


    public class ReadOnlyModuleDictionary<TModule> : IReadOnlyModuleDictionary<TModule>
        where TModule : notnull
    {
        private readonly MutableModuleDictionary<TModule> dictionary;

        public ReadOnlyModuleDictionary(MutableModuleDictionary<TModule> dictionary) 
            => this.dictionary = dictionary;
        
        public bool IsReadOnly => true;
        
        public bool Contains(TModule item) => this.dictionary.Contains(item);
        public bool Contains(string key) => this.dictionary.Contains(key);
        public bool Contains(XNamespace key) => this.dictionary.Contains(key);
        
        public TModule? this[string key] => this.dictionary[(XNamespace)key];
        public TModule? this[XNamespace key] => this.dictionary[key];
        
        public TModule? GetValueOrDefault(string key) => this.dictionary.GetValueOrDefault(key);
        public TModule? GetValueOrDefault(XNamespace key) => this.dictionary.GetValueOrDefault(key);
        
        
        public bool TryGetValue(string key, [NotNullWhen(true)] out TModule? item) 
            => this.dictionary.TryGetValue(key, out item);
        public bool TryGetValue(XNamespace key, [NotNullWhen(true)] out TModule? item) 
            => this.dictionary.TryGetValue(key, out item);
        
        public int Count => this.dictionary.Count;
        public void Dispose() => this.dictionary.Dispose();
        public IEnumerator<TModule> GetEnumerator() => this.dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        TModule IReadOnlyList<TModule>.this[int index] => ((IReadOnlyList<TModule>)this.dictionary)[index]!; // Null-forgiving
    }
    
    
    public abstract class MutableModuleDictionary<TModule> : IModuleDictionary<TModule>
        where TModule : notnull
    {
        private int revision;
        private readonly ReaderWriterLockSlim lockObject = new ();
        protected abstract XNamespace GetNamespaceFromModule(TModule module);
        protected abstract string GetNameFromModule(TModule module);
        
        private readonly Dictionary<XNamespace, TModule> byNamespace = new();
        private readonly Dictionary<string, TModule> byName = new();
        private readonly List<TModule> byIndex = new();
        protected bool IsDisposed { get; private set; }

        public void ClearInternal()
        {
            try
            {
                this.lockObject.EnterWriteLock();
                Interlocked.Increment(ref this.revision);
                this.byNamespace.Clear();
                this.byName.Clear();
                this.byIndex.Clear();
            }
            finally
            {
                this.lockObject.ExitWriteLock();
            }
        }

        private bool TryRemoveInternal(TModule item, XNamespace ns, string name)
        {
            try
            {
                this.lockObject.EnterWriteLock();
                Interlocked.Increment(ref this.revision);
                var success = true;
                success &= this.byNamespace.Remove(ns);
                success &= this.byName.Remove(name);
                success &= this.byIndex.Remove(item);
                return success;
            }
            finally
            {
                this.lockObject.ExitWriteLock();
            }
        }
        private Result TryAddInternal(TModule item, XNamespace ns, string name)
        {
            item = item ?? throw new ArgumentNullException(nameof(item));
            lockObject.EnterUpgradeableReadLock();
            try
            {
                if (!this.DoesNotContainWithoutLock(item, ns, name).Try(out var error))
                {
                    return error;
                }
                lockObject.EnterWriteLock();
                Interlocked.Increment(ref this.revision);
                try
                {
                    AddWithoutLock(item, ns, name);
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }
                return Result.SuccessfulResult;
            }
            finally
            {
                lockObject.ExitUpgradeableReadLock();
            }
        }

        private Result DoesNotContainWithoutLock(TModule item, XNamespace ns, string name)
        {
            if (this.byIndex.Contains(item))
                return Result.CreateError($"Item {item} already exists in list.");
            if(this.byNamespace.ContainsKey(ns))
                return Result.CreateError($"Duplicate namespace {ns}");
            if(this.byName.ContainsKey(name))
                return Result.CreateError($"Duplicate name {name}");
            return Result.SuccessfulResult;
        }

        private void AddWithoutLock(TModule item, XNamespace ns, string name)
        {
            this.byNamespace.Add(ns, item);
            this.byName.Add(name, item);
            this.byIndex.Add(item);
        }

        
        private bool ContainsInternal(TModule item)
        {
            lockObject.EnterReadLock();
            try
            {
                return this.byIndex.Contains(item);
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
        private bool ContainsInternal(XNamespace item)
        {
            lockObject.EnterReadLock();
            try
            {
                return this.byNamespace.ContainsKey(item);
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }        
        private bool ContainsInternal(string item)
        {
            lockObject.EnterReadLock();
            try
            {
                return this.byName.ContainsKey(item);
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
        private TModule? GetValueOrDefaultInternal(XNamespace key)
        {
            lockObject.EnterReadLock();
            try
            {
                return this.byNamespace.GetValueOrDefault(key);
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }        
        private TModule? GetValueOrDefaultInternal(string key)
        {
            lockObject.EnterReadLock();
            try
            {
                return this.byName.GetValueOrDefault(key);
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
        private TModule? GetValueOrDefaultInternal(int index)
        {
            lockObject.EnterReadLock();
            try
            {
                return index >= 0 && index < this.byIndex.Count ? this.byIndex[index] : default;
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
        
        private int GetCountInternal()
        {
            lockObject.EnterReadLock();
            try
            {
                return this.byIndex.Count;
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        private IEnumerator<TModule> GetEnumeratorInternal() => new Enumerator(this);

        public void Clear() => this.ClearInternal();
        public Result TryAdd(TModule item) => TryAddInternal(item, GetNamespaceFromModule(item), GetNameFromModule(item));
        public void Add(TModule item)
        {
            if (!TryAddInternal(item, GetNamespaceFromModule(item), GetNameFromModule(item)).Try(out var error))
            {
                throw new (error.ErrorMessage ?? "Unexpected error occurred.");
            }
        }
        
        public void AddRange(IEnumerable<TModule> items)
        {
            try
            {
                this.lockObject.EnterWriteLock();
                foreach (var item in items)
                {
                    AddWithoutLock(item, GetNamespaceFromModule(item), GetNameFromModule(item));
                }
            }
            finally
            {
                this.lockObject.ExitWriteLock();
            }
        }

        public bool Remove(TModule item)
            => TryRemoveInternal(item, GetNamespaceFromModule(item), GetNameFromModule(item));
        Result IKeyedCollection<XNamespace, TModule>.TryAdd(TModule item) => this.TryAdd(item);
        void IKeyedCollection<XNamespace, TModule>.Add(TModule item) => this.Add(item);
        bool IKeyedCollection<XNamespace, TModule>.Remove(TModule item) => this.Remove(item);
        public bool Contains(TModule item) => this.ContainsInternal(item);
        public bool Contains(XNamespace key) => this.ContainsInternal(key);
        public bool Contains(string key) => this.ContainsInternal(key);
        public bool IsReadOnly => false;
        public TModule? this[XNamespace key] => this.GetValueOrDefaultInternal(key);
        public TModule? this[string key] => this.GetValueOrDefaultInternal(key);
        public TModule? GetValueOrDefault(XNamespace key) => this.GetValueOrDefaultInternal(key);
        public TModule? GetValueOrDefault(string key) => this.GetValueOrDefaultInternal(key);
        public bool TryGetValue(XNamespace key, [NotNullWhen(true)] out TModule? item)
        {
            item = GetValueOrDefaultInternal(key);
            return item is not null;
        }

        public bool TryGetValue(string key, [NotNullWhen(true)]out TModule? item) 
        {
            item = GetValueOrDefaultInternal(key);
            return item is not null;
        }
        public int Count => GetCountInternal();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumeratorInternal();
        public IEnumerator<TModule> GetEnumerator() => this.GetEnumeratorInternal();
        TModule IReadOnlyList<TModule>.this[int index] => this.GetValueOrDefaultInternal(index)!; // Null-forgiving operator

        protected virtual void Dispose(bool disposing)
        {
            this.IsDisposed = true;
            if (disposing)
            {
                this.lockObject.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class Enumerator : IEnumerator<TModule>
        {
            private readonly MutableModuleDictionary<TModule> dict;
            private int? revision;
            private int index;
            public Enumerator(MutableModuleDictionary<TModule> dict) => this.dict = dict;

            public bool MoveNext()
            {
                if(this.revision is not null)
                    this.CheckRevisionAndDisposed();
                else
                {
                    this.revision = GetRevision();
                    this.index = -1;
                }
                ++this.index;
                return this.index >= 0 && this.index < this.dict.Count;
            }
            
            private TModule GetCurrent()
            {
                CheckRevisionAndDisposed();
                this.dict.lockObject.EnterReadLock();
                try
                {
                    return this.dict.byIndex[this.index] ?? throw new InvalidOperationException();
                }
                finally
                {
                    this.dict.lockObject.ExitReadLock();
                }
            }
            
            private void CheckRevisionAndDisposed()
            {
                if (this.dict.IsDisposed)
                    throw new ObjectDisposedException(nameof(this.dict));
                if (this.revision != this.GetRevision())
                    throw new ("Collection was modified; enumeration operation may not execute");
            }

            private int GetRevision()
            {
                if (this.dict.IsDisposed)
                    throw new ObjectDisposedException(nameof(this.dict));
                this.dict.lockObject.EnterReadLock();
                try
                {
                    return this.dict.revision;
                }
                finally
                {
                    this.dict.lockObject.ExitReadLock();
                }
            }

            public void Reset() => this.revision = null;
            public TModule Current => this.GetCurrent(); 
            object? IEnumerator.Current => this.Current;
            public void Dispose() { }
        }

    }
}
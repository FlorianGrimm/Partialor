namespace Partialor;

public readonly struct PartialNoValue {}

public readonly struct PartialValue<T> {
    private readonly bool _HasValue = false;
    private readonly T _Value = default(T)!;
    
    public PartialValue() { _HasValue = false; _Value = default(T)!; }
    public PartialValue(bool hasValue, T value) { _HasValue = hasValue; _Value = value; }
    public PartialValue(T value) { _HasValue = true; _Value = value; }

    public bool HasValue => this._HasValue;

    public T Value {
        get {
            if (this._HasValue) {
                return this._Value;
            } else {
                throw new NullReferenceException("Value");
            }
        }
    }

    public readonly bool TryGetValue([MaybeNullWhen(false)] out T value) {
        if (_HasValue) {
            value = this._Value!;
            return true;
        } else {
            value = default(T);
            return false;
        }
    }

    public readonly T GetValueOrDefault(in T value) {
        if (this._HasValue) {
            return this._Value;
        } else {
            return value;
        }
    }

    public static PartialValue<T> NoValue() => new();
    
    public static PartialValue<T> WithValue(T value) => new(value);
    
    public static implicit operator PartialValue<T>(T value) { 
        return new(value); 
    }

    public static implicit operator PartialValue<T>(PartialNoValue noValue) { 
        return new(); 
    }
}

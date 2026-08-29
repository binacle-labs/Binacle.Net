namespace Binacle.FluxResults;

public readonly struct FluxUnion<T0, T1> : IFluxUnion
    where T0 : notnull
    where T1 : notnull
{
    private enum Flux
    {
        T0,
        T1
    }

    private readonly Flux flux;
    private readonly T0? t0;
    private readonly T1? t1;

    private FluxUnion(
        Flux flux,
        T0? t0 = default,
        T1? t1 = default
    )
    {
        this.flux = flux;
        this.t0 = t0;
        this.t1 = t1;
    }

    public static implicit operator FluxUnion<T0, T1>(T0 t0) => new(Flux.T0, t0: t0);
    public static implicit operator FluxUnion<T0, T1>(T1 t1) => new(Flux.T1, t1: t1);

    public object GetValue()
    {
        return this.flux switch
        {
            Flux.T0 => this.t0!,
            Flux.T1 => this.t1!,
            _ => throw new InvalidOperationException("Invalid result value.")
        };
    }

    public Type GetValueType()
    {
        return this.flux switch
        {
            Flux.T0 => typeof(T0),
            Flux.T1 => typeof(T1),
            _ => throw new InvalidOperationException("Invalid result value.")
        };
    }

    public TResult Match<TResult>(
        Func<T0, TResult> t0Func,
        Func<T1, TResult> t1Func
    )
    {
        return this.flux switch
        {
            Flux.T0 => t0Func(this.t0!),
            Flux.T1 => t1Func(this.t1!),
            _ => throw new InvalidOperationException("Invalid result value.")
        };
    }
}
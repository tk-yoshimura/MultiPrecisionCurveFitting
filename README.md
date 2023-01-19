# MultiPrecisionCurveFitting 
  Float Multi Precision Curve Fitting - linear, polynomial, pade, arbitrary function

## Requirement
.NET 6.0

AVX2 suppoted CPU. (Intel:Haswell(2013)-, AMD:Excavator(2015)-)

## Install

[Download DLL](https://github.com/tk-yoshimura/MultiPrecisionCurveFitting/releases)  
[Download Nuget package](https://www.nuget.org/packages/tyoshimura.multiprecision.curvefitting/)

- Import MultiPrecision(https://github.com/tk-yoshimura/MultiPrecision)
- Import MultiPrecisionAlgebra(https://github.com/tk-yoshimura/MultiPrecisionAlgebra)
- To install, just import the DLL.
- This library does not change the environment at all.

## Usage

```csharp
MultiPrecision<Pow2.N8>[] xs = (new MultiPrecision<Pow2.N8>[1024]).Select((_, i) => MultiPrecision<Pow2.N8>.Div(i, 1024)).ToArray();
MultiPrecision<Pow2.N8>[] ys = xs.Select(v => MultiPrecision<Pow2.N8>.Cos(v) - 0.25).ToArray();

PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

bool callback(Vector<Pow2.N8> parameters) {
    MultiPrecision<Pow2.N8> norm = fitter.Error(parameters).Norm;

    Console.WriteLine(parameters);
    Console.WriteLine(norm);

    bool is_continue = norm > 1e-3;

    return is_continue;
};

Vector<Pow2.N8> parameters = fitter.ExecuteFitting(iter: 256, iter_callback: callback);

Console.WriteLine($"Numer : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[..fitter.Numer]}");
Console.WriteLine($"Denom : {(Vector<Pow2.N8>)((MultiPrecision<Pow2.N8>[])parameters)[fitter.Numer..]}");
```

See also: [Tests](https://github.com/tk-yoshimura/MultiPrecisionCurveFitting/tree/main/MultiPrecisionCurveFittingTest)

## Licence
[MIT](https://github.com/tk-yoshimura/MultiPrecisionCurveFitting/blob/master/LICENSE)

## Author

[T.Yoshimura](https://github.com/tk-yoshimura)


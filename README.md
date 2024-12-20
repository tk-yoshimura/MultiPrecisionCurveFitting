# MultiPrecisionCurveFitting 
  MultiPrecision Curve Fitting - linear, polynomial, pade, arbitrary function

## Requirement
.NET 8.0
AVX2 suppoted CPU. (Intel:Haswell(2013)-, AMD:Excavator(2015)-)  
[MultiPrecision](https://github.com/tk-yoshimura/MultiPrecision)  
[MultiPrecisionAlgebra](https://github.com/tk-yoshimura/MultiPrecisionAlgebra)

## Install

[Download DLL](https://github.com/tk-yoshimura/MultiPrecisionCurveFitting/releases)  
[Download Nuget](https://www.nuget.org/packages/tyoshimura.multiprecision.curvefitting/)

## Usage

```csharp
MultiPrecision<Pow2.N8>[] xs = Vector<Pow2.N8>.Arange(1024) / 1024;
MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => MultiPrecision<Pow2.N8>.Cos(x) - 0.25, xs);

PadeFitter<Pow2.N8> fitter = new(xs, ys, intercept: 0.75, numer: 4, denom: 3);

Vector<Pow2.N8> parameters = fitter.Fit();

Console.WriteLine($"Numer : {parameters[..fitter.Numer]}");
Console.WriteLine($"Denom : {parameters[fitter.Numer..]}");

Assert.AreEqual(0.75, fitter.Regress(0, parameters));

for (int i = 0; i < xs.Length; i++) {
    Assert.IsTrue(MultiPrecision<Pow2.N8>.Abs(ys[i] - fitter.Regress(xs[i], parameters)) < 1e-5,
        $"\nexpected : {ys[i]}\n actual  : {fitter.Regress(xs[i], parameters)}"
    );
}
```

See also: [Tests](https://github.com/tk-yoshimura/MultiPrecisionCurveFitting/tree/main/MultiPrecisionCurveFittingTest)

## Licence
[MIT](https://github.com/tk-yoshimura/MultiPrecisionCurveFitting/blob/master/LICENSE)

## Author

[T.Yoshimura](https://github.com/tk-yoshimura)


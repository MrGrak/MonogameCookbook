// dotnet run -c Release -f net8.0 --filter "*"

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithId("Scalar").WithEnvironmentVariable("DOTNET_EnableHWIntrinsic", "0").AsBaseline())
    .AddJob(Job.Default.WithId("Vector128").WithEnvironmentVariable("DOTNET_EnableAVX2", "0").WithEnvironmentVariable("DOTNET_EnableAVX512F", "0"))
    .AddJob(Job.Default.WithId("Vector256").WithEnvironmentVariable("DOTNET_EnableAVX512F", "0"))
    .AddJob(Job.Default.WithId("Vector512"));
BenchmarkSwitcher.FromAssembly(typeof(Tests).Assembly).Run(args, config);

[HideColumns("Error", "StdDev", "Median", "RatioSD", "EnvironmentVariables", "value")]
public class Tests
{
    private readonly byte[] _data = Enumerable.Repeat((byte)123, 999).Append((byte)42).ToArray();

    [Benchmark]
    [Arguments((byte)42)]
    public bool Find(byte value) => Contains(_data, value);

    private static unsafe bool Contains(ReadOnlySpan<byte> haystack, byte needle)
    {
        if (Vector128.IsHardwareAccelerated && haystack.Length >= Vector128<byte>.Count)
        {
            ref byte current = ref MemoryMarshal.GetReference(haystack);

            if (Vector512.IsHardwareAccelerated && haystack.Length >= Vector512<byte>.Count)
            {
                Vector512<byte> target = Vector512.Create(needle);
                ref byte endMinusOneVector = ref Unsafe.Add(ref current, haystack.Length - Vector512<byte>.Count);
                do
                {
                    if (Vector512.EqualsAny(target, Vector512.LoadUnsafe(ref current)))
                        return true;

                    current = ref Unsafe.Add(ref current, Vector512<byte>.Count);
                }
                while (Unsafe.IsAddressLessThan(ref current, ref endMinusOneVector));

                if (Vector512.EqualsAny(target, Vector512.LoadUnsafe(ref endMinusOneVector)))
                    return true;
            }
            else if (Vector256.IsHardwareAccelerated && haystack.Length >= Vector256<byte>.Count)
            {
                Vector256<byte> target = Vector256.Create(needle);
                ref byte endMinusOneVector = ref Unsafe.Add(ref current, haystack.Length - Vector256<byte>.Count);
                do
                {
                    if (Vector256.EqualsAny(target, Vector256.LoadUnsafe(ref current)))
                        return true;

                    current = ref Unsafe.Add(ref current, Vector256<byte>.Count);
                }
                while (Unsafe.IsAddressLessThan(ref current, ref endMinusOneVector));

                if (Vector256.EqualsAny(target, Vector256.LoadUnsafe(ref endMinusOneVector)))
                    return true;
            }
            else
            {
                Vector128<byte> target = Vector128.Create(needle);
                ref byte endMinusOneVector = ref Unsafe.Add(ref current, haystack.Length - Vector128<byte>.Count);
                do
                {
                    if (Vector128.EqualsAny(target, Vector128.LoadUnsafe(ref current)))
                        return true;

                    current = ref Unsafe.Add(ref current, Vector128<byte>.Count);
                }
                while (Unsafe.IsAddressLessThan(ref current, ref endMinusOneVector));

                if (Vector128.EqualsAny(target, Vector128.LoadUnsafe(ref endMinusOneVector)))
                    return true;
            }
        }
        else
        {
            for (int i = 0; i < haystack.Length; i++)
                if (haystack[i] == needle)
                    return true;
        }

        return false;
    }
}
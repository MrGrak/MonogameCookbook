# Array of Structs Examples

In this folder you'll find a variety of different AoS examples that I've written over the years. Generally, each .cs file attempts to explore some concept related to AoS, explained in comments. AoS stands for "Array of Structs", and it's arguably the most useful data structure for high performance programming (like games). This is for several reasons:

1. The data in an array of structs is arranged contiguously. 

This means the data is side by side when stored in ram, tightly packed. Compare this tight packing of data with data that is spread out across different memory locations. Each time we fetch data, we have to wait for the data to arrive in the register before the cpu can actually perform work on it. If that data is fetched from ram, it's very slow. If that data is fetched from l1 cache, it's very fast. Cachelines fetch in chunks of 64 bytes, so it's very probable that multiple structs will be fetched with one cacheline, due to being tightly packed.

2. AoS works well with hardware prefetchers.

The goal is to reduce how long we have to wait for data to arrive by making the data tightly packed and accessing the data in a very predictable way, which allows the hardware prefetchers to recognize the access pattern and start prefetching the data for us (in a stream), reducing how long we have to wait. Learn abnd love the Hardware prefetcher.

3. Structs are smaller than classes.

In addition to prefetching, storing data in structs is smaller than storing data in classes, since each class is heap allocated (in C#) and therefore requires additional data to be stored for the garbage collector and clr. A single reference takes either 4 bytes on 32-bit processors or 8 bytes on 64-bit processors. Imagine you have a particle that is 8 bytes in size. As a struct, the size in memory is 8 bytes. As a class, the size in memory doubles to 16 bytes. The larger the instances are, the fewer you can fit into a cacheline, the fewer you can fit into L1, L2, L3 caches. Smaller data is faster. Structs are smaller than classes.

4. Structs in Arrays accessed linearly (using a for loop) can be vectorized with (relative) ease.

This is not a hot take, as vectorization/SIMD requires data to be arranged this way. So, if you arrange your data in arrays, as structs, and then discover that you need the code to be even faster, you can vectorize it. If you're particularly clever, you can get the compiler to auto-vectorize your AoS (assuming you have observed the rules of vectorization). Multithreaded SIMD is as fast as we can execute code currently (unless you want to use compute shaders on the gpu). Please note that SIMD places a lot of trust in the architecture, and may not be the best solution when actually implemented unless you're really good at it and know the hardware.

"But what about ECS? How does it compare to AoS?"

A proper ECS will use structs to represent components, and arrange them into arrays that are sequentially accessed, for the reasons mentioned above. A good ECS will also multithread this accesses. AoS can be thought of as the "low level" structure powering the "high level" component abstraction that is the C in ECS. In this way, AoS exists inside of ECS, and when leveraged properly is the reason why ECS is fast and useful. Adding, removing, and filtering components isn't useful if the entire process is slow (might as well just use an AoS at that point). So, if your ECS doesn't provide performance over an AoS - then it's time to find a proper, good ECS. 

//MrGrak	
	
# An Example of using static classes, threads, and tasks

This folder contains a console program that tests several different ways of combining static classes, threads, and tasks. We start with Test 1, which is just Updating() an array of structs with some basic work, using a for loop, like you might see in a video game update loop. This serves as a baseline to compare against. When the number of structs is small, this baseline Test 1 is the fastest solution (this is due to the overhead of threads and tasks). When the number of structs grows large, other Tests become the faster solutions. 

The next step to making these solutions even faster would be to vectorize (SIMD) them, but unfortunately C# has limited support for such operations so these examples are "as fast as they can be". 

Note that each test writes a log file detailing info about the particle state, the frame, and the elapsed ticks. This is done so that we can check particle state and verify that the data has been mutated as we expect. 

The motivation for writing these examples and tests stems from feedback from other programmers that static classes and threads/tasks should not be mixed together (for reasons that fail to meet my quality and logic standards). My hope is that by seeing how to combine these concepts, programmers can use them appropriately, and stop complaining about them.

//MrGrak



	
	
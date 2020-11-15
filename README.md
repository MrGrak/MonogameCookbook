# Monogame Cookbook

A collection of code samples and examples for Monogame developers.  
You may find them useful or useless.  Use as you wish.  
These are written in vs2019, with monogame 3.8.  
//MrGrak



# ToDo/Goals:

Brief overview and what Monogame is, how it's used, etc
The art that will be used in the example game built, and
the example game that will be built. All the tutorials are
written into the code, so just reading the comments is the
tutorial. this means assets/ is just a bunch of .cs files,
with a proper license, so other devs can use the code.

this may take several years, so bear with me.


	
# Time
	-Variable vs fixed timesteps
	-Timing just your cpu side code
	-Timing the gpu and frames per second
	-Timing methods using benchmark.net
	-Notes on pre optimization
	
# Fonts + Text
	-Hello World
	-Bitmap fonts vs ttf fonts
	-Creating a custom bitmap font
	
# The All Powerful Sprite
	-What sprites are, quads, mg's sprite batcher
	-Drawing your first sprite, basic sprite class and struct
	-Using sprites for text, multi language
	-Using sprites for ui, windows, buttons
	-Using sprites for game objects: particles, projectiles, actors, objects
	-Using sprites for logos, visual effects
	-Using atlases, and what draw calls are

# Screens + ScreenManager
	-Creating base screen class, and public static manager
	-Opening, Closing screens, and exit actions
	-A boilerplate screen based game system overview (layering, dialogs, etc)
	-Creating a game screen and pause screen
	-Creating screens each time vs static instances

# Game Input
	-why it's useful to have a layer of input abstraction
	-a global input manager example for quickly prototyping
	-struct based instances, and their advantages
	-Recording input with structs on a list
	-pausing the game, unpausing the game
	
# Physics
	-Writing a basic simplified newtonian physics system
	-Working with 2 dimensional gravity, mass, and friction
	-Create a hero square that player can move around, in basic level
	-Adding jump and jump sound effect too
	
# Collisions
	-AABB and Radius Check Algorithms
	-Checking one rec against another
	-Checking one circle against another
	-Checking millions of recs, and spatial partitioning
	-Checking a 2d point within rec or radius
	
	-Using AABB checks for a hero square in level
	-Expanding the level to include collision objects
	
	-Using AABB checks for uis, a basic button
	-Animating the button open, closed, interaction
	-Building a basic level select screen
	-Loading levels, saving levels, formats (xml, json, binary, c#)
	
# Animation
	-Coding a hero walk cycle, with sample sprite art
	-Coding a hero jump, fall, land cycle.
	-Coding a hero dash, punch, and idle cycle.
	-Coding an enemy idle, walk, death cycle (basic ai)

# Interaction
	-Adding hero to game screen
	-Adding enemies, spawning enemies
	
# Projectiles
	-Melee vs Moving vs Area vs Global
	-Creating an aos projectile pool, with sprites
	-Spawning different types of projectiles
	-Adding projectiles and interactions to example game
	
# Particles
	-Adding basic particles in an aos style pool system
	
# Sound, Music, and Audio
	-Why you should use wavs, and how to design with them
	-Using sound effect instances for everything
	-Adding basic sound effects to the game screen example (level music, soundfx)
	-Using Fmod or Wwise or 3rd party
	
# Structure of Data
	-Classes, Structs, Pointers, DoD, AoS, Speed and Size
	-Class vs Struct
	-A Class Particle System
	-A Struct Particle System
	-Designing with compact structs in arrays for optimization
	
# Designing Codebases
	-Create an example game out of the example screen
	-Should have a few levels, screens, enemies, be compact like smww
	
	
	
	
	
	
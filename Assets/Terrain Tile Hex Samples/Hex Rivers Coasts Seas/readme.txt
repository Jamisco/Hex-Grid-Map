Hex Rivers, Coasts, Seas: Painted 2D Terrain

2D art assets by David Baumgart

email: dgbaumgart@gmail.com
webpage: dgbaumgart.com
twitter: @dgbaumgart

Last Update: v1.0 on 28 May 2021

Description:
This is a set of lovingly hand-painted 2D terrain hex tiles of rivers, coastlines, and sea variations suitable for an overworld, RPG, strategy game, or boardgame-like visuals. 

In this set you will find:
- 2 new types of water tile x 4 variations each (lake, calm ocean) = 8 total
- 12 water-themed feature tiles (bridges, islands, harbor, shipwreck)
- 83 river tiles (including variations)
- 6 river mouth sprites (to connect rivers to seas over coasts)
- 126 coast tiles (including coast variations)
- 3 new "below" tiles for underwater/land transitions under coastlines
- all tiles are painted at 256x384 pixels so that trees, hills, and mountains can overlap the tiles behind 
- also includes 42 non-tile decor sprites (lakes, bridges, docks, islands, ships, etc.)

Do note that these tiles don't need any particular version of Unity to use. The tiles are drawn as transparent background png sprites that can be implemented in a variety of ways.


Usage:
These hexformat terrain tiles are intended to be drawn so that tiles in rows lower on the screen are drawn overtop of tiles in rows higher on the screen. This will ensure that trees, hills, and mountains will appear to stand out.

***

Common questions & answers to them:

Q. I want to use a grid where 1 Unity unit equals one tile but your examples use 1.28 unity units per tile.
A. I based this off the default setting of 100 pixels per Unity unit for sprites. If you want tiles to work correctly on a whole-number grid, just select all the tiles and set the "Pixels Per Unit" to 128, then adjust units everywhere as appropriate.

Q. The tiles are overlapping incorrectly, for example my trees are underneath the grasslands on the tile "above" them. 
A. Pack the sprites used by the tiles into one sprite atlas. This seems to make the sprite renderer happy, though you have to be in play mode to see this work correctly.

Q. Something is drawn badly or there's a hole in a sprite or something is misnamed!
A. Please email me and I will fix it! I want these assets to be as perfect as possible, and I do make mistakes. Rest assured that mistakes drive me as crazy as they do you, so I'll take care of them quickly.

Q. Can I have these at a higher resolution?
A. What you're getting is the resolution I painted them at. You might be able to use one of those clever upscaling programs keyed toward digital paintings to upscale the tiles with good results.

*** 

Example Scene Notes

DISCLAIMER: I am not a professionally trained programmer. If you examine the example code I created, you will quickly see this. This example scene is provided to show that it is possible to make these tiles work dynamically in a game.

I do not recommend you use my code (though you may), and I want to make it clear that what you are buying with this asset set is NOT code support for a dynamic terrain system because I am not qualified to provide that and I am not charging you for it. This code is not optimized and there are many places where you can and should do things in a more "Unity-friendly" manner.

With that said, let me share some thoughts: 

- The way these tiles all work together can be fairly complex. I've tried to create assets that allow *maximum versatility* at the cost of more elegant, but limited, design. Instead of making all the design choices for you my goal is to provide you with as many tools as I reasonably can so you can make your own choices - within the limits of the Unity's default Tilemap implementation.
- I've tried to comment my code extensively to show you my intentions. Doubtless you will spot ways to make it more efficient and effective and I encourage you to explore those possibilities.
- I advise that you use Unity's "Scriptable Tile" system as explained here: https://docs.unity3d.com/Manual/Tilemap-ScriptableTiles.html to implement coasts and rivers.
- The "mountain top sprite over river" workaround is truly egregious and for that I apologize. Unity's tilemap system (created after my first sets of tiles were drawn, I should note! ahem ahem.) handles layering in a way which does not work nicely with how I have handled rivers and 'tall' tile overlapping. For these to work together without visual artifacts, compromises had to be made. If you figure out a way to handle this case which is more elegant and doesn't require complex code, I'd love to hear it.

* * *

Changelog:
1.0: 
	- First Release
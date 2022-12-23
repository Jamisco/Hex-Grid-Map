Hex Cold Lands: Painted 2D Terrain

2D art assets by David Baumgart

email: dgbaumgart@gmail.com
twitter: @dgbaumgart
webpage: dgbaumgart.com


Last Update: v1.1 in March 2021

Description:
This is a set of lovingly hand-painted 2D terrain hex tiles of cold and snow-covered land suitable for an overworld, a strategy game, or boardgame-like visuals. 
- 13 tile types x 4 variations + 20 additional location tiles = 72 tiles total 
- includes 40 non-tile terrain decor sprites that can be overlaid anywhere
- painted at 256x384 pixels so that trees, hills, and mountains can overlap the tiles behind 

Terrain types included: 
- snow-covered mountains (+ 4 variants with caves) 
- snowfields
- forest (pine, snow transition, snow-covered + clearing for each type)
- rocky hills (bare, snow transition, snow-covered + cave for each type)
- cold plains (bare, snow transition, snow-covered + pond for each type)
- ocean with icebergs
- cold dirt
- also some ruins, a giant skeleton, an ice palace, and logging camps

Please note that this package does not actually require any particular version of Unity; you can use these in pretty much any version of any application that supports sprites. The sprites are saved as individual tiles in a separate folder. I've removed the one-image "tilesheet' images because I suspect these are redundant; if you want them, just sent me an email. Unity and other applications have built-in sprite atlasing support, and I recommend using that for optimizing if you find the need to do so.

Usage:
These hexagonal terrain tiles are intended to be drawn so that tiles in rows lower on the screen are drawn overtop of tiles in rows higher on the screen. This will ensure that trees, hills, and mountains will appear to stand out and over the tiles 'behind' them.


Usage:
These square-format terrain tiles are intended to be drawn so that tiles in rows lower on the screen are drawn overtop of tiles in rows higher on the screen. This will ensure that trees, hills, and mountains will appear to stand out. You can make this a square grid or a staggered square grid to get a hex-grid like effect.


Unity Tilemaps:

Unity Tilemaps were introduced in Unity 2017. You do not need this version of Unity to use my assets, but I do provide a sample scene which implements them using the Unity Tilemap system. The Tilemap section of the Unity manual is a good starting place if you have not worked with Unity's Tilemap system: https://docs.unity3d.com/Manual/class-Tilemap.html

Again, you do not NEED to use Unity's built-in Tilemap system. While it is efficient and optimized, it has certain limitations that make certain use-cases of my tiles awkward to work around. If you're making straightforward maps, you'll be fine. If you're doing anything really complex using sorting layers within a tilemap, you will have a bad time unless you get into some advanced implementation - or roll your own system. 

Q. I want to use a grid where 1 Unity unit equals one cell but your examples use 2.56 unity units per cell.
A. I based this off the default setting of 100 pixels per Unity unit for imported images. If you want tiles to work on a whole-number grid, just select all the tiles and set the "Pixels Per Unit" to 256, then your grid can use its default settings of one unit per cell.  

Q. The tiles are overlapping incorrectly, for example my trees are underneath the grasslands on the tile "above" them. 
A. Pack the tile sprites into one sprite atlas. This seems to make the sprite renderer happier, though you have to be in play mode to see this work correctly.
A2. Check the "Sort Order" setting under "Tilemap Renderer". Make sure it is Top Left or Top Right. 
A3. Check the "Mode" setting under "Tilemap Renderer". Sometimes it helps to set this to "Individual" rather than "Chunk".
A4. Check the anchor settings of 1. the sprites, 2. the tilemap palette prefab, and 3. the tilemap renderer. (See next Q.)

Q. My tiles are not aligning to the grid
A1. Make sure that the "Tile Anchor" is set correctly. Depending on how the sprite anchor is set this can vary. In my example scene, sprites are set to anchor "bottom" and the Tile Anchor on the Tilemap is set to (0, -2/3). Underground-type hex sprites are set to (0.5, 2/3) so that they look like they're an underground section to the hex cell you select. Experiment a bit with these; there's no right answer, you just have to be consistent with yourself.
A2. Click "Open Prefab" in the tile palette parent object and edit the prefab. Check the "Tile Anchor" and "Tilemap Renderer" settings.
A3. The hex under-tiles are weirdly aligned, yes. Sorry! This is so they can fit on the same hex grid as the normal terrain layer. I am open to alternative solutions for this.

Q. Something is drawn badly or there's a hole in a sprite or something is misnamed!
A. Please email me and I will fix it! I want these assets to be as perfect as possible, and I do make mistakes. Rest assured that mistakes drive me as crazy as they do you, so I'll take care of them very quickly.

Q. Can you provide a version of the hexes with flat tops and bottoms?
A. Short answer: no, and it is not going to happen. This would require redrawing every single asset, hundreds of tiles. This would be hundreds of hours of work for little additional benefit. I made a decision about hex orientation in 2016 and now I'm locked in.

Q. Why aren't the hexes geometrically correct with equal length sides and 120 degree corners?
A. I made the vertices snap to a 32x32 pixel grid and the edges have a run:rise ratio of 2:1 because it makes everything a million times easier for me to work with using raster graphics. The alternative, perfect hexes, would be a significant increase in work to look as good as the hexes do now.


Future:
If these prove popular, I may wish to do further tiles to add to this set, and further types of tilesets like this. Feel free to email me with ideas! Please also let me know if *anything* is amiss or incorrect and I will fix it as quickly as possible.

Changelog:
1.0: 2016 07 01 
	- First Release
1.1: 2021 03 12 
	- Cleaned up stray pixels
	- Improved all hex tile art
	- Added 9 new hexes
	- Added 40 new decor sprites
	- Added example scene

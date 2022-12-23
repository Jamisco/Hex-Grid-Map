Tiles and Hexes: 2D Painted Terrain Samples

2D art assets by David Baumgart

email: dgbaumgart@gmail.com
twitter: @dgbaumgart
webpage: dgbaumgart.com


Last Update: v1.0 2021 March 20

Description:
This is a free sample set of lovingly hand-painted 2D terrain hex and square tiles so you can try the tile system out and see if my assets fit your needs. These tiles are suitable for a game overworld, a strategy game, or boardgame-like visuals. 
- includes 10 hex tiles + 2 underground hex pieces, 10 square tiles + 2 underground pieces
- painted at 256x384 pixels so that trees, hills, and mountains can overlap the tiles behind 

This package does not actually require any particular version of Unity; you can use these in pretty much any version of any application that supports sprites. The sprites are saved as individual png files.


Usage:

These terrain tiles are intended to be drawn so that tiles in rows lower on the screen are drawn overtop of tiles in rows higher on the screen. This will ensure that trees, hills, and mountains will appear to stand out and over the tiles 'behind' them. The square tiles can be drawn on a simple grid or on a staggered hex-style. 


Do you want to buy more tiles?
Wonderful! My Unity Asset Store page can be found here: https://assetstore.unity.com/publishers/13841
Or if you're looking for individual packages, check out these links:

Hexes:
"Painted 2D Terrain Hexes: Basic Set"            - http://u3d.as/mZG
"Hex Cold Lands: Painted 2D Terrain"             - http://u3d.as/wkt
"Painted 2D Location Hexes: Medieval-Fantasy"    - http://u3d.as/oZ7
"Painted 2D Terrain Hexes: Deserts"              - http://u3d.as/KZb
"Painted 2D Terrain Hexes: Tropics and Wetlands" - http://u3d.as/12zZ
"Painted 2D Terrain Hexes: Volcanic Wastes"      - http://u3d.as/12Gr

Square Tiles:
"Painted 2D Terrain Hexes: Basic Set"		 - http://u3d.as/juT
"Cold Lands: Painted 2D Terrain Tiles"           - http://u3d.as/wku
"Painted 2D Location Tiles: Medieval-Fantasy"    - http://u3d.as/oZ7
"Painted 2D Terrain Tiles: Deserts"              - http://u3d.as/KsD
"Painted 2D Terrain Tiles: Tropics and Wetlands" - http://u3d.as/12zU 
"Painted 2D Terrain Tiles: Volcanic Wastes"      - http://u3d.as/12Gt


Example Scene & Unity Tilemaps:

Unity Tilemaps were introduced in Unity 2017. You do not need this version of Unity to use my assets, but I do provide a sample scene which implements them using the Unity Tilemap system. The Tilemap section of the Unity manual is a good starting place if you have not worked with Unity's Tilemap system: https://docs.unity3d.com/Manual/class-Tilemap.html

Again, you do not NEED to use Unity's built-in Tilemap system. While it is efficient and optimized, it has certain limitations that make certain use-cases of my tiles awkward to work around. If you're making straightforward maps, you'll be fine. If you're doing anything really complex using sorting layers within a tilemap, you will have a bad time unless you get into some advanced implementation or roll your own system. 

Q. I want to use a grid where 1 Unity unit equals one cell but your examples use 2.56 unity units per cell.
A. I based this off the default setting of 100 pixels per Unity unit for imported images. If you want tiles to work on a whole-number grid, just select all the tiles and set the "Pixels Per Unit" to 256, then your grid can use its default settings of one unit per cell.  

Q. The tiles are overlapping incorrectly, for example my trees are underneath the grasslands on the tile "above" them. 
A. Pack the tile sprites into one sprite atlas. This seems to make the sprite renderer happier, though you have to be in play mode to see this work correctly.
A2. Check the "Sort Order" setting under "Tilemap Renderer". Make sure it is Top Left or Top Right. 
A3. Check the "Mode" setting under "Tilemap Renderer". Sometimes it helps to set this to "Individual" rather than "Chunk".
A4. Check the anchor settings of A. the sprites, B. the tilemap palette prefab, and C. the tilemap renderer. (See next Q.)

Q. My tiles are not aligning to the grid
A1. Make sure that the "Tile Anchor" is set correctly. Depending on how the sprite anchor is set this can vary. In my example scene, sprites are set to anchor "bottom" and the Tile Anchor on the Tilemap is set to (0, -2/3). Underground-type hex sprites are set to (0.5, 2/3) so that they look like they're an underground section to the hex cell you select. Experiment a bit with these; there's no right answer, you just have to be consistent with yourself and your grid settings.
A2. Click "Open Prefab" in the tile palette parent object and edit the prefab. Check the "Tile Anchor" and "Tilemap Renderer" settings.
A3. The hex under-tiles are weirdly aligned, yes. Sorry! This is so they can fit on the same hex grid as the normal terrain layer. I am open to alternative solutions for this.

Q. Something is drawn badly or there's a hole in a sprite or something is misnamed!
A. Please email me and I will fix it! I want these assets to be as perfect as possible, and I do make mistakes. Rest assured that mistakes drive me as crazy as they do you, so I'll take care of them very quickly.

Q. Can you provide a version of the hexes with flat tops and bottoms?
A. Short answer: no, and it is not going to happen. This would require redrawing every single asset, hundreds of tiles. This would be hundreds of hours of work for little additional benefit. I made a decision about hex orientation in 2016 and now I'm locked in. Sorry!

Q. Why aren't the hexes geometrically correct with equal length sides and 120 degree corners?
A. I made the vertices snap to a 32x32 pixel grid and the edges have a run:rise ratio of 2:1 because it makes everything a million times easier for me to work with using raster graphics. The alternative, perfect hexes, would be a significant increase in work to look as good as the hexes do now.

Q. Coastlines?
A. I'm going to make a package for these. They are however difficult to implement with Unity's default tilemap system.

Q. Rivers?
A. I'm going to make a package for these. They are however difficult to implement with Unity's default tilemap system.

Q. Roads? 
A. They're in the "Medieval Fantasy Locations" asset pack.

Q. Borderless, tiling tiles?
A. Considering it. Non-trivial work. Requires rethinking how most of the terrain works, ie. you can't just make a forest or mountain texture that tiles seamlessly without putting a lot of effort into making the edges look good somehow.


Future:
Please also let me know if *anything* is amiss or incorrect and I will fix it as quickly as possible.


Changelog:
1.0: 2021 03 20 
	- First Release

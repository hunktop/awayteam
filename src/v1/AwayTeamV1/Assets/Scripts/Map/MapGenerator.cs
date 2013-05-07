using System;
using UnityEngine;

public class MapGenerator
{
	/*
	 * Let me get this out of the way:
	 * YES, I know that I use width and height wrong in
	 * arrays, because the first coordinate in arrays is
	 * for rows but I pretend that they're for
	 * height. Whatever.
	 */
	private int totalCols;
	private int totalRows;
	private int maxBuildings;
	//private TileProperties[] tileSet;
	private TileProperties[,] map;
	private tileType[,] mapAreas;
	private System.Random rand;
	private int insanityLevel = 100;
	private int gutter = 2;
	private float buildingScale = 2.0f;
	private int mapSeed;
	
	public TileProperties[,] GenerateMap(int mapWidth, int mapHeight, int desiredNumberOfBuildings = 3, int randomSeed = -1)
	{
		totalCols = mapWidth;
		totalRows = mapHeight;
		maxBuildings = desiredNumberOfBuildings;
		mapSeed = randomSeed;
		if (mapSeed == -1) {
			rand = new System.Random();
		} else {
			rand = new System.Random(mapSeed);
		}
		//this.tileSet = tileSet;
		var map = new TileProperties[totalRows, totalCols];
		var mapAreas = assignAreas();
		map = areasToTiles(mapAreas);
		return map;
	}
	
	private enum tileType
	{
		outdoor,
		indoor,
		wall,
		spacer,
		door
	}
	
	private tileType[,] assignAreas()
	{
		var areas = new tileType[totalRows,totalCols];
		for (int rr=0; rr<totalRows; rr++)
		{
			for (int cc=0; cc<totalCols; cc++)
			{
				areas[rr,cc] = tileType.outdoor;
			}
		}
		var sanityCheck = 0;
		var buildingsPlaced = 0;
		var randWid = 0;
		var randHei = 0;
		var randStartRow = 0;
		var randStartCol = 0;
		var placeFree = true;
		while (sanityCheck < insanityLevel && buildingsPlaced < maxBuildings)
		{
			randWid = rand.Next(gutter, (int)Math.Ceiling((totalCols-gutter)/buildingScale));
			randHei = rand.Next(gutter, (int)Math.Ceiling((totalRows-gutter)/buildingScale));
			randStartRow = rand.Next(0,totalRows-randHei-1);
			randStartCol = rand.Next(0,totalCols-randWid-1);
			placeFree = true;
			for (int rr=randStartRow; rr<=randStartRow+randHei; rr++)
			{
				for (int cc=randStartCol; cc<=randStartCol+randWid; cc++)
				{
					if (areas[rr,cc] == tileType.wall || areas[rr,cc] == tileType.indoor || areas[rr,cc] == tileType.spacer)
					{
						placeFree = false;
						break;
					}
				}
			}
			if (placeFree)
			{
				var doorsPlaced = 0;
				for (int rr=randStartRow; rr<=randStartRow+randHei; rr++)
				{
					for (int cc=randStartCol; cc<=randStartCol+randWid; cc++)
					{
						if (rr==randStartRow || rr==randStartRow+randHei || cc==randStartCol || cc==randStartCol+randWid)
						{
							areas[rr,cc] = tileType.wall;
							if ((rr==randStartRow || rr==randStartRow+randHei) ^ (cc==randStartCol || cc==randStartCol+randWid)) {
								if (rand.NextDouble() < 0.2 && doorsPlaced < 2) {
									areas[rr,cc] = tileType.door;
									doorsPlaced++;
								}
							}
						} else
						{
							areas[rr,cc] = tileType.indoor;
						}
					}
				}
				for (int rr=randStartRow; rr<=randStartRow+randHei; rr++)
				{
					if (randStartCol>0)
					{
						areas[rr,randStartCol-1] = tileType.spacer;
					}
					if (randStartCol+randWid+1 < totalCols-1)
					{
						areas[rr,randStartCol+randWid+1] = tileType.spacer;
					}
				}
				for (int cc=randStartCol; cc<=randStartCol+randWid; cc++)
				{
					if (randStartRow>0)
					{
						areas[randStartRow-1,cc] = tileType.spacer;
					}
					if (randStartRow+randHei+1 < totalRows-1)
					{
						areas[randStartRow+randHei+1,cc] = tileType.spacer;
					}
				}
				buildingsPlaced++;
			}
			sanityCheck++;
		}
		return areas;
	}
	
	private TileProperties[,] areasToTiles(tileType[,] mapAreas)
	{
		var mapTiles = new TileProperties[totalRows,totalCols];
		var thisTile = StaticTiles.GrassTile;
		for (int ii=0; ii<totalRows; ii++)
		{
			for (int jj=0; jj<totalCols; jj++)
			{
				switch (mapAreas[ii,jj])
				{
					case tileType.outdoor:
						thisTile = (rand.NextDouble() < 0.7) ? StaticTiles.GrassTile : StaticTiles.ForestTile; // eventually I need to make these into bunched forests
						break;
					case tileType.indoor:
						thisTile = StaticTiles.FloorTile;
						break;
					case tileType.door:
						thisTile = StaticTiles.FloorTile;
						break;
					case tileType.wall:
						thisTile = StaticTiles.WallTile;
						break;
					default:
						thisTile = StaticTiles.GrassTile;
						break;
				}
				mapTiles[ii,jj] = thisTile;
			}
		}
		return mapTiles;
	}
}
